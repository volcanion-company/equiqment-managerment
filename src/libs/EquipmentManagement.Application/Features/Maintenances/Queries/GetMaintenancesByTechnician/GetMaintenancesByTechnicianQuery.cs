using EquipmentManagement.Application.Features.Maintenances.DTOs;
using MediatR;

namespace EquipmentManagement.Application.Features.Maintenances.Queries.GetMaintenancesByTechnician;

/// <summary>
/// Query to get work queue for a specific technician
/// </summary>
public class GetMaintenancesByTechnicianQuery : IRequest<IEnumerable<MaintenanceRequestDto>>
{
    /// <summary>
    /// Technician ID
    /// </summary>
    public string TechnicianId { get; set; } = string.Empty;

    /// <summary>
    /// Show only active (Pending + InProgress) maintenances. Default is true.
    /// </summary>
    public bool ActiveOnly { get; set; } = true;
}
