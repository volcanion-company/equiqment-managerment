using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Repositories;
using EquipmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Infrastructure.Repositories;

public class EquipmentRepository : Repository<Equipment>, IEquipmentRepository
{
    public EquipmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Equipment?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Code == code, cancellationToken);
    }

    public async Task<(IEnumerable<Equipment> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? type = null,
        string? status = null,
        string? keyword = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(e => e.Type == type);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (Enum.TryParse<Domain.Enums.EquipmentStatus>(status, true, out var statusEnum))
            {
                query = query.Where(e => e.Status == statusEnum);
            }
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(e => 
                e.Code.Contains(keyword) || 
                e.Name.Contains(keyword) ||
                (e.Description != null && e.Description.Contains(keyword)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
