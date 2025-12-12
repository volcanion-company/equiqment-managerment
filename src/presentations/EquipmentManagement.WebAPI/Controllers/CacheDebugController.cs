using EquipmentManagement.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManagement.WebAPI.Controllers;

/// <summary>
/// Debug controller for testing cache operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Debug")]
public class CacheDebugController : ControllerBase
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CacheDebugController> _logger;

    public CacheDebugController(ICacheService cacheService, ILogger<CacheDebugController> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    /// <summary>
    /// Set a test cache value
    /// </summary>
    [HttpPost("set")]
    public async Task<IActionResult> SetCache([FromQuery] string key, [FromQuery] string value)
    {
        try
        {
            var testData = new TestCacheData { Value = value };
            await _cacheService.SetAsync(key, testData);
            _logger.LogInformation("Cache set: {Key} = {Value}", key, value);
            return Ok(new { message = "Cache set successfully", key, value });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get a test cache value
    /// </summary>
    [HttpGet("get")]
    public async Task<IActionResult> GetCache([FromQuery] string key)
    {
        try
        {
            var result = await _cacheService.GetAsync<TestCacheData>(key);
            if (result == null)
            {
                return NotFound(new { message = "Cache not found", key });
            }
            return Ok(new { key, value = result.Value });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Remove cache by prefix
    /// </summary>
    [HttpDelete("remove-prefix")]
    public async Task<IActionResult> RemoveByPrefix([FromQuery] string prefix)
    {
        try
        {
            _logger.LogInformation("Attempting to remove cache by prefix: {Prefix}", prefix);
            await _cacheService.RemoveByPrefixAsync(prefix);
            return Ok(new { message = "Cache removed by prefix", prefix });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache by prefix");
            return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    /// <summary>
    /// Test scenario: Set multiple keys and remove by prefix
    /// </summary>
    [HttpPost("test-scenario")]
    public async Task<IActionResult> TestScenario()
    {
        try
        {
            var results = new List<string>();

            // Set test keys
            await _cacheService.SetAsync("equipments_1", new TestCacheData { Value = "Equipment 1" });
            results.Add("Set: equipments_1");

            await _cacheService.SetAsync("equipments_2", new TestCacheData { Value = "Equipment 2" });
            results.Add("Set: equipments_2");

            await _cacheService.SetAsync("equipments_3", new TestCacheData { Value = "Equipment 3" });
            results.Add("Set: equipments_3");

            await _cacheService.SetAsync("warehouse_1", new TestCacheData { Value = "Warehouse 1" });
            results.Add("Set: warehouse_1");

            // Verify they exist
            var eq1 = await _cacheService.GetAsync<TestCacheData>("equipments_1");
            var eq2 = await _cacheService.GetAsync<TestCacheData>("equipments_2");
            var eq3 = await _cacheService.GetAsync<TestCacheData>("equipments_3");
            var wh1 = await _cacheService.GetAsync<TestCacheData>("warehouse_1");

            results.Add($"Before delete - equipments_1: {(eq1 != null ? "EXISTS" : "NULL")}");
            results.Add($"Before delete - equipments_2: {(eq2 != null ? "EXISTS" : "NULL")}");
            results.Add($"Before delete - equipments_3: {(eq3 != null ? "EXISTS" : "NULL")}");
            results.Add($"Before delete - warehouse_1: {(wh1 != null ? "EXISTS" : "NULL")}");

            // Remove by prefix
            _logger.LogInformation("Calling RemoveByPrefixAsync with 'equipments_'");
            await _cacheService.RemoveByPrefixAsync("equipments_");
            results.Add("Called: RemoveByPrefixAsync('equipments_')");

            // Verify after delete
            eq1 = await _cacheService.GetAsync<TestCacheData>("equipments_1");
            eq2 = await _cacheService.GetAsync<TestCacheData>("equipments_2");
            eq3 = await _cacheService.GetAsync<TestCacheData>("equipments_3");
            wh1 = await _cacheService.GetAsync<TestCacheData>("warehouse_1");

            results.Add($"After delete - equipments_1: {(eq1 != null ? "STILL EXISTS ❌" : "DELETED ✓")}");
            results.Add($"After delete - equipments_2: {(eq2 != null ? "STILL EXISTS ❌" : "DELETED ✓")}");
            results.Add($"After delete - equipments_3: {(eq3 != null ? "STILL EXISTS ❌" : "DELETED ✓")}");
            results.Add($"After delete - warehouse_1: {(wh1 != null ? "PRESERVED ✓" : "DELETED ❌")}");

            return Ok(new { results });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in test scenario");
            return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    private class TestCacheData
    {
        public string Value { get; set; } = string.Empty;
    }
}
