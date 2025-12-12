using EquipmentManagement.Application.Common.Interfaces;
using EquipmentManagement.Domain.Repositories;
using EquipmentManagement.Infrastructure.Persistence;
using EquipmentManagement.Infrastructure.Persistence.Interceptors;
using EquipmentManagement.Infrastructure.Repositories;
using EquipmentManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace EquipmentManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add interceptor
        services.AddSingleton<UtcDateTimeInterceptor>();

        // Database
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var interceptor = serviceProvider.GetRequiredService<UtcDateTimeInterceptor>();
            options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                .AddInterceptors(interceptor);
        });

        // Redis
        var redisConnectionString = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            // Register IConnectionMultiplexer for direct Redis access
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configurationOptions = ConfigurationOptions.Parse(redisConnectionString);
                return ConnectionMultiplexer.Connect(configurationOptions);
            });

            // Register Redis distributed cache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "EquipmentManagement_";
            });
        }

        // Repositories
        services.AddScoped<IEquipmentRepository, EquipmentRepository>();
        services.AddScoped<IWarehouseItemRepository, WarehouseItemRepository>();
        services.AddScoped<IWarehouseTransactionRepository, WarehouseTransactionRepository>();
        services.AddScoped<IAssignmentRepository, AssignmentRepository>();
        services.AddScoped<IAuditRecordRepository, AuditRecordRepository>();
        services.AddScoped<IMaintenanceRequestRepository, MaintenanceRequestRepository>();
        services.AddScoped<ILiquidationRequestRepository, LiquidationRequestRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<IQRCodeService, QRCodeService>();

        return services;
    }
}
