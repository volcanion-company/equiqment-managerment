using EquipmentManagement.Domain.Entities;

namespace EquipmentManagement.Domain.Repositories;

public interface ILiquidationRequestRepository : IRepository<LiquidationRequest>
{
    Task<IEnumerable<LiquidationRequest>> GetByEquipmentIdAsync(Guid equipmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<LiquidationRequest>> GetPendingRequestsAsync(CancellationToken cancellationToken = default);
    Task<(IEnumerable<LiquidationRequest> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        bool? isApproved = null,
        CancellationToken cancellationToken = default);
}
