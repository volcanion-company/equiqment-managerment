using EquipmentManagement.Domain.Repositories;
using EquipmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace EquipmentManagement.Infrastructure.Repositories;

public class UnitOfWork(
    ApplicationDbContext context,
    IEquipmentRepository equipments,
    IWarehouseItemRepository warehouseItems,
    IWarehouseTransactionRepository warehouseTransactions,
    IAssignmentRepository assignments,
    IAuditRecordRepository auditRecords,
    IMaintenanceRequestRepository maintenanceRequests,
    ILiquidationRequestRepository liquidationRequests) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    public IEquipmentRepository Equipments { get; } = equipments;
    public IWarehouseItemRepository WarehouseItems { get; } = warehouseItems;
    public IWarehouseTransactionRepository WarehouseTransactions { get; } = warehouseTransactions;
    public IAssignmentRepository Assignments { get; } = assignments;
    public IAuditRecordRepository AuditRecords { get; } = auditRecords;
    public IMaintenanceRequestRepository MaintenanceRequests { get; } = maintenanceRequests;
    public ILiquidationRequestRepository LiquidationRequests { get; } = liquidationRequests;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        context.Dispose();
    }
}
