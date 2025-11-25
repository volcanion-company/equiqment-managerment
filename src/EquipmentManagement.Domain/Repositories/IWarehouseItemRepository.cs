using EquipmentManagement.Domain.Entities;

namespace EquipmentManagement.Domain.Repositories;

public interface IWarehouseItemRepository : IRepository<WarehouseItem>
{
    Task<WarehouseItem?> GetByEquipmentTypeAsync(string equipmentType, CancellationToken cancellationToken = default);
    Task<IEnumerable<WarehouseItem>> GetLowStockItemsAsync(CancellationToken cancellationToken = default);
}
