using EquipmentManagement.Domain.Entities;

namespace EquipmentManagement.Domain.Repositories;

public interface IAuditRecordRepository : IRepository<AuditRecord>
{
    Task<IEnumerable<AuditRecord>> GetByEquipmentIdAsync(Guid equipmentId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<AuditRecord> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid? equipmentId = null,
        CancellationToken cancellationToken = default);
}
