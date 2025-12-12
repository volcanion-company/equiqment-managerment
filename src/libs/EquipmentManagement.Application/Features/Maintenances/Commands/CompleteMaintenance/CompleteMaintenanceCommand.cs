using MediatR;

namespace EquipmentManagement.Application.Features.Maintenances.Commands.CompleteMaintenance;

/// <summary>
/// Command to complete a maintenance work
/// </summary>
public class CompleteMaintenanceCommand : IRequest<Unit>
{
    /// <summary>
    /// Maintenance request ID
    /// </summary>
    public Guid MaintenanceRequestId { get; set; }

    /// <summary>
    /// Technician ID completing the work (for validation)
    /// </summary>
    public string TechnicianId { get; set; } = string.Empty;

    /// <summary>
    /// Total cost of maintenance (required)
    /// </summary>
    public decimal Cost { get; set; }

    /// <summary>
    /// Completion notes describing work done
    /// </summary>
    public string? CompletionNotes { get; set; }

    /// <summary>
    /// Indicate if equipment still needs further maintenance
    /// </summary>
    public bool StillNeedsMaintenance { get; set; }
}
