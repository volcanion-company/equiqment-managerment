using MediatR;

namespace EquipmentManagement.Application.Features.Maintenances.Commands.CancelMaintenance;

/// <summary>
/// Command to cancel a maintenance request
/// </summary>
public class CancelMaintenanceCommand : IRequest<Unit>
{
    /// <summary>
    /// Maintenance request ID
    /// </summary>
    public Guid MaintenanceRequestId { get; set; }

    /// <summary>
    /// Reason for cancellation (required)
    /// </summary>
    public string CancellationReason { get; set; } = string.Empty;

    /// <summary>
    /// Person who cancelled the request
    /// </summary>
    public string? CancelledBy { get; set; }
}
