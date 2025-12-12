namespace EquipmentManagement.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    IEquipmentRepository Equipments { get; }
    IWarehouseItemRepository WarehouseItems { get; }
    IWarehouseTransactionRepository WarehouseTransactions { get; }
    IAssignmentRepository Assignments { get; }
    IAuditRecordRepository AuditRecords { get; }
    IMaintenanceRequestRepository MaintenanceRequests { get; }
    ILiquidationRequestRepository LiquidationRequests { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
