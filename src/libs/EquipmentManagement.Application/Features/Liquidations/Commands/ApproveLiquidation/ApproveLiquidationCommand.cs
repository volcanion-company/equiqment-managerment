using MediatR;

namespace EquipmentManagement.Application.Features.Liquidations.Commands.ApproveLiquidation;

/// <summary>
/// Command to approve a liquidation request
/// </summary>
public class ApproveLiquidationCommand : IRequest<Unit>
{
    /// <summary>
    /// Liquidation request ID
    /// </summary>
    public Guid LiquidationRequestId { get; set; }

    /// <summary>
    /// Approver's user ID (manager/authorized person)
    /// </summary>
    public string ApprovedBy { get; set; } = string.Empty;

    /// <summary>
    /// Liquidation value (selling price/disposal value)
    /// </summary>
    public decimal LiquidationValue { get; set; }

    /// <summary>
    /// Optional approval notes
    /// </summary>
    public string? ApprovalNotes { get; set; }
}
