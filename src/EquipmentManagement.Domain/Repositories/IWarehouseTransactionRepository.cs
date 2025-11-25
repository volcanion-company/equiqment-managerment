using EquipmentManagement.Domain.Entities;

namespace EquipmentManagement.Domain.Repositories;

public interface IWarehouseTransactionRepository : IRepository<WarehouseTransaction>
{
    Task<IEnumerable<WarehouseTransaction>> GetByWarehouseItemIdAsync(Guid warehouseItemId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<WarehouseTransaction> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid? warehouseItemId = null,
        CancellationToken cancellationToken = default);
}
