using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Repositories;
using EquipmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Infrastructure.Repositories;

public class WarehouseItemRepository(ApplicationDbContext context) : Repository<WarehouseItem>(context), IWarehouseItemRepository
{
    public async Task<WarehouseItem?> GetByEquipmentTypeAsync(string equipmentType, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(w => w.EquipmentType == equipmentType, cancellationToken);
    }

    public async Task<IEnumerable<WarehouseItem>> GetLowStockItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(w => w.Quantity <= w.MinThreshold)
            .ToListAsync(cancellationToken);
    }
}
