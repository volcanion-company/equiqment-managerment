using EquipmentManagement.Application.Features.Liquidations.DTOs;
using MediatR;

namespace EquipmentManagement.Application.Features.Liquidations.Queries.GetLiquidationRequests;

/// <summary>
/// Query to get paginated list of liquidation requests
/// </summary>
public class GetLiquidationRequestsQuery : IRequest<GetLiquidationRequestsQueryResult>
{
    /// <summary>
    /// Page number (default: 1)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Page size (default: 10, max: 100)
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Filter by approval status (null = all, true = approved, false = rejected)
    /// </summary>
    public bool? IsApproved { get; set; }
}
