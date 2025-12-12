using EquipmentManagement.Domain.Entities;

namespace EquipmentManagement.Domain.Repositories;

public interface IAssignmentRepository : IRepository<Assignment>
{
    Task<IEnumerable<Assignment>> GetByEquipmentIdAsync(Guid equipmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Assignment>> GetActiveAssignmentsByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Assignment> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid? equipmentId = null,
        string? userId = null,
        CancellationToken cancellationToken = default);
}
