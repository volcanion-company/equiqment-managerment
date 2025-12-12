using MediatR;

namespace EquipmentManagement.Application.Features.Maintenances.Commands.StartMaintenance;

/// <summary>
/// Command to start a maintenance work
/// </summary>
public class StartMaintenanceCommand : IRequest<Unit>
{
    /// <summary>
    /// Maintenance request ID
    /// </summary>
    public Guid MaintenanceRequestId { get; set; }

    /// <summary>
    /// Technician ID starting the work (for validation)
    /// </summary>
    public string TechnicianId { get; set; } = string.Empty;

    /// <summary>
    /// Optional notes when starting
    /// </summary>
    public string? StartNotes { get; set; }
}
