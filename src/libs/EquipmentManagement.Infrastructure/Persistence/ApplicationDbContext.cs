using System.Reflection;
using EquipmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Equipment> Equipments => Set<Equipment>();
    public DbSet<WarehouseItem> WarehouseItems => Set<WarehouseItem>();
    public DbSet<WarehouseTransaction> WarehouseTransactions => Set<WarehouseTransaction>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<AuditRecord> AuditRecords => Set<AuditRecord>();
    public DbSet<MaintenanceRequest> MaintenanceRequests => Set<MaintenanceRequest>();
    public DbSet<LiquidationRequest> LiquidationRequests => Set<LiquidationRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Configure all DateTime properties to be stored as UTC in PostgreSQL
        configurationBuilder.Properties<DateTime>()
            .HaveConversion<UtcDateTimeConverter>();
        
        configurationBuilder.Properties<DateTime?>()
            .HaveConversion<UtcNullableDateTimeConverter>();
        
        base.ConfigureConventions(configurationBuilder);
    }
}
