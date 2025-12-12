using MediatR;

namespace EquipmentManagement.Application.Features.Maintenances.Commands.UpdateMaintenance;

/// <summary>
/// Command to update basic maintenance request information
/// </summary>
public class UpdateMaintenanceCommand : IRequest<Unit>
{
    /// <summary>
    /// Maintenance request ID
    /// </summary>
    public Guid MaintenanceRequestId { get; set; }

    /// <summary>
    /// Updated description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Updated notes
    /// </summary>
    public string? Notes { get; set; }
}
