using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Enums;
using EquipmentManagement.Domain.Repositories;
using EquipmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Infrastructure.Repositories;

public class AssignmentRepository : Repository<Assignment>, IAssignmentRepository
{
    public AssignmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Assignment>> GetByEquipmentIdAsync(Guid equipmentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.EquipmentId == equipmentId)
            .OrderByDescending(a => a.AssignedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Assignment>> GetActiveAssignmentsByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.AssignedToUserId == userId && a.Status == AssignmentStatus.Assigned)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Assignment> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid? equipmentId = null,
        string? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (equipmentId.HasValue)
        {
            query = query.Where(a => a.EquipmentId == equipmentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(userId))
        {
            query = query.Where(a => a.AssignedToUserId == userId);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.AssignedDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
