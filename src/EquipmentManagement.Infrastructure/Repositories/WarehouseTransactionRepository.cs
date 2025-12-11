using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Repositories;
using EquipmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Infrastructure.Repositories;

public class WarehouseTransactionRepository(ApplicationDbContext context) : Repository<WarehouseTransaction>(context), IWarehouseTransactionRepository
{
    public async Task<IEnumerable<WarehouseTransaction>> GetByWarehouseItemIdAsync(Guid warehouseItemId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.WarehouseItemId == warehouseItemId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<WarehouseTransaction> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid? warehouseItemId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (warehouseItemId.HasValue)
        {
            query = query.Where(t => t.WarehouseItemId == warehouseItemId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.TransactionDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
