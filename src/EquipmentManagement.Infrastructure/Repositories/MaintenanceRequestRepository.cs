using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Repositories;
using EquipmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Infrastructure.Repositories;

public class MaintenanceRequestRepository(ApplicationDbContext context) : Repository<MaintenanceRequest>(context), IMaintenanceRequestRepository
{
    public async Task<IEnumerable<MaintenanceRequest>> GetByEquipmentIdAsync(Guid equipmentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.EquipmentId == equipmentId)
            .OrderByDescending(m => m.RequestDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MaintenanceRequest>> GetByTechnicianIdAsync(string technicianId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.TechnicianId == technicianId)
            .OrderByDescending(m => m.RequestDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<MaintenanceRequest> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid? equipmentId = null,
        string? technicianId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (equipmentId.HasValue)
        {
            query = query.Where(m => m.EquipmentId == equipmentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(technicianId))
        {
            query = query.Where(m => m.TechnicianId == technicianId);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(m => m.RequestDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
