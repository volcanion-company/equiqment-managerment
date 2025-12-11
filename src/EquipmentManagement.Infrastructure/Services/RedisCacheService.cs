using System.Text.Json;
using EquipmentManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace EquipmentManagement.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer? _connectionMultiplexer;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly string _instancePrefix;

    public RedisCacheService(
        IDistributedCache cache, 
        ILogger<RedisCacheService> logger,
        IConnectionMultiplexer? connectionMultiplexer = null)
    {
        _cache = cache;
        _logger = logger;
        _connectionMultiplexer = connectionMultiplexer;
        
        // Get instance prefix from RedisCache if available
        if (cache is RedisCache redisCache)
        {
            var options = redisCache.GetType()
                .GetProperty("Options", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(redisCache) as RedisCacheOptions;
            _instancePrefix = options?.InstanceName ?? string.Empty;
        }

        _instancePrefix ??= "EquipmentManagement_";

        _logger.LogInformation("RedisCacheService initialized with prefix: {Prefix}", _instancePrefix);
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        var cachedValue = await _cache.GetStringAsync(key, cancellationToken);

        if (string.IsNullOrEmpty(cachedValue))
        {
            return null;
        }

        return JsonSerializer.Deserialize<T>(cachedValue);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        var serializedValue = JsonSerializer.Serialize(value);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30)
        };

        await _cache.SetStringAsync(key, serializedValue, options, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        if (_connectionMultiplexer == null)
        {
            _logger.LogWarning("Cannot remove keys by prefix: IConnectionMultiplexer is null. Prefix: {Prefix}", prefix);
            return;
        }

        if (!_connectionMultiplexer.IsConnected)
        {
            _logger.LogWarning("Cannot remove keys by prefix: Redis is not connected. Prefix: {Prefix}", prefix);
            return;
        }

        try
        {
            var endpoints = _connectionMultiplexer.GetEndPoints();
            if (endpoints == null || endpoints.Length == 0)
            {
                _logger.LogWarning("No Redis endpoints available");
                return;
            }

            var server = _connectionMultiplexer.GetServer(endpoints.First());
            var database = _connectionMultiplexer.GetDatabase();

            // Construct the full pattern with instance prefix
            var pattern = $"{_instancePrefix}{prefix}*";
            
            _logger.LogDebug("Searching for Redis keys with pattern: {Pattern}", pattern);

            // Use SCAN instead of KEYS for better performance and safety
            var keysFound = 0;
            var keysDeleted = 0;
            
            await foreach (var key in server.KeysAsync(pattern: pattern, pageSize: 250))
            {
                keysFound++;
                var deleted = await database.KeyDeleteAsync(key);
                if (deleted)
                {
                    keysDeleted++;
                }
            }

            _logger.LogInformation(
                "RemoveByPrefixAsync completed. Pattern: {Pattern}, Keys found: {KeysFound}, Keys deleted: {KeysDeleted}", 
                pattern, keysFound, keysDeleted);
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogError(ex, "Redis connection error while removing keys by prefix: {Prefix}", prefix);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing keys by prefix: {Prefix}", prefix);
            throw;
        }
    }
}
