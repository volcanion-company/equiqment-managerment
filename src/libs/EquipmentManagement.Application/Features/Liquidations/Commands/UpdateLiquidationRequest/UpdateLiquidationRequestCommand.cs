using MediatR;

namespace EquipmentManagement.Application.Features.Liquidations.Commands.UpdateLiquidationRequest;

/// <summary>
/// Command to update a liquidation request (only pending requests)
/// </summary>
public class UpdateLiquidationRequestCommand : IRequest<Unit>
{
    /// <summary>
    /// Liquidation request ID
    /// </summary>
    public Guid LiquidationRequestId { get; set; }

    /// <summary>
    /// Updated liquidation value estimate
    /// </summary>
    public decimal? LiquidationValue { get; set; }

    /// <summary>
    /// Updated notes/reason for liquidation
    /// </summary>
    public string? Note { get; set; }
}
