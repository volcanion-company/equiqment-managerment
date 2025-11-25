using EquipmentManagement.Domain.Repositories;
using EquipmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace EquipmentManagement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(
        ApplicationDbContext context,
        IEquipmentRepository equipments,
        IWarehouseItemRepository warehouseItems,
        IWarehouseTransactionRepository warehouseTransactions,
        IAssignmentRepository assignments,
        IAuditRecordRepository auditRecords,
        IMaintenanceRequestRepository maintenanceRequests,
        ILiquidationRequestRepository liquidationRequests)
    {
        _context = context;
        Equipments = equipments;
        WarehouseItems = warehouseItems;
        WarehouseTransactions = warehouseTransactions;
        Assignments = assignments;
        AuditRecords = auditRecords;
        MaintenanceRequests = maintenanceRequests;
        LiquidationRequests = liquidationRequests;
    }

    public IEquipmentRepository Equipments { get; }
    public IWarehouseItemRepository WarehouseItems { get; }
    public IWarehouseTransactionRepository WarehouseTransactions { get; }
    public IAssignmentRepository Assignments { get; }
    public IAuditRecordRepository AuditRecords { get; }
    public IMaintenanceRequestRepository MaintenanceRequests { get; }
    public ILiquidationRequestRepository LiquidationRequests { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
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
        _context.Dispose();
    }
}
