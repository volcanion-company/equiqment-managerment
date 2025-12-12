using EquipmentManagement.Application.Features.Liquidations.DTOs;
using MediatR;

namespace EquipmentManagement.Application.Features.Liquidations.Queries.GetLiquidationById;

/// <summary>
/// Query to get liquidation request detail by ID
/// </summary>
public class GetLiquidationByIdQuery : IRequest<LiquidationRequestDto>
{
    /// <summary>
    /// Liquidation request ID
    /// </summary>
    public Guid LiquidationRequestId { get; set; }
}
