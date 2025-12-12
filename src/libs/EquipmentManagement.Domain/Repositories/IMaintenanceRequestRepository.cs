using EquipmentManagement.Domain.Entities;

namespace EquipmentManagement.Domain.Repositories;

public interface IMaintenanceRequestRepository : IRepository<MaintenanceRequest>
{
    Task<IEnumerable<MaintenanceRequest>> GetByEquipmentIdAsync(Guid equipmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MaintenanceRequest>> GetByTechnicianIdAsync(string technicianId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<MaintenanceRequest> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid? equipmentId = null,
        string? technicianId = null,
        CancellationToken cancellationToken = default);
}
