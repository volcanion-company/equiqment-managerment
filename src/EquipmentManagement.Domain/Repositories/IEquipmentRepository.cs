using EquipmentManagement.Domain.Entities;

namespace EquipmentManagement.Domain.Repositories;

public interface IEquipmentRepository : IRepository<Equipment>
{
    Task<Equipment?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Equipment> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? type = null,
        string? status = null,
        string? keyword = null,
        CancellationToken cancellationToken = default);
}
