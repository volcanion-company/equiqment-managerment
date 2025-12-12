using EquipmentManagement.Application.Features.Maintenances.DTOs;
using MediatR;

namespace EquipmentManagement.Application.Features.Maintenances.Queries.GetMaintenanceById;

/// <summary>
/// Query to get maintenance request detail by ID
/// </summary>
public class GetMaintenanceByIdQuery : IRequest<MaintenanceRequestDto>
{
    /// <summary>
    /// Maintenance request ID
    /// </summary>
    public Guid MaintenanceRequestId { get; set; }
}
