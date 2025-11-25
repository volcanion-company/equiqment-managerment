using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Repositories;
using EquipmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Infrastructure.Repositories;

public class AuditRecordRepository : Repository<AuditRecord>, IAuditRecordRepository
{
    public AuditRecordRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<AuditRecord>> GetByEquipmentIdAsync(Guid equipmentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.EquipmentId == equipmentId)
            .OrderByDescending(a => a.CheckDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<AuditRecord> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid? equipmentId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (equipmentId.HasValue)
        {
            query = query.Where(a => a.EquipmentId == equipmentId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.CheckDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
