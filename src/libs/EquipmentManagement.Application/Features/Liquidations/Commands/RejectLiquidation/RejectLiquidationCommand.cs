using MediatR;

namespace EquipmentManagement.Application.Features.Liquidations.Commands.RejectLiquidation;

/// <summary>
/// Command to reject a liquidation request
/// </summary>
public class RejectLiquidationCommand : IRequest<Unit>
{
    /// <summary>
    /// Liquidation request ID
    /// </summary>
    public Guid LiquidationRequestId { get; set; }

    /// <summary>
    /// Reviewer's user ID (manager/authorized person)
    /// </summary>
    public string RejectedBy { get; set; } = string.Empty;

    /// <summary>
    /// Reason for rejection (required)
    /// </summary>
    public string RejectionReason { get; set; } = string.Empty;
}
