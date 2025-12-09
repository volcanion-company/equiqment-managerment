using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Repositories;
using EquipmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Infrastructure.Repositories;

public class LiquidationRequestRepository(ApplicationDbContext context) : Repository<LiquidationRequest>(context), ILiquidationRequestRepository
{
    public async Task<IEnumerable<LiquidationRequest>> GetByEquipmentIdAsync(Guid equipmentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.EquipmentId == equipmentId)
            .OrderByDescending(l => l.RequestDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<LiquidationRequest>> GetPendingRequestsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => !l.IsApproved)
            .OrderByDescending(l => l.RequestDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<LiquidationRequest> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        bool? isApproved = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (isApproved.HasValue)
        {
            query = query.Where(l => l.IsApproved == isApproved.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(l => l.RequestDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
