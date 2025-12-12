using MediatR;

namespace EquipmentManagement.Application.Features.Maintenances.Commands.AssignTechnician;

/// <summary>
/// Command to assign a technician to a maintenance request
/// </summary>
public class AssignTechnicianCommand : IRequest<Unit>
{
    /// <summary>
    /// Maintenance request ID
    /// </summary>
    public Guid MaintenanceRequestId { get; set; }

    /// <summary>
    /// Technician ID to assign
    /// </summary>
    public string TechnicianId { get; set; } = string.Empty;

    /// <summary>
    /// Optional notes about the assignment
    /// </summary>
    public string? AssignmentNotes { get; set; }
}
