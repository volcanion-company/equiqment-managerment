using EquipmentManagement.Application.Features.Liquidations.DTOs;
using MediatR;

namespace EquipmentManagement.Application.Features.Liquidations.Queries.GetPendingLiquidations;

/// <summary>
/// Query to get all pending liquidation requests (not yet approved or rejected)
/// </summary>
public class GetPendingLiquidationsQuery : IRequest<IEnumerable<LiquidationRequestDto>>
{
}
