using EquipmentManagement.Application.Features.Liquidations.DTOs;

namespace EquipmentManagement.Application.Features.Liquidations.Queries.GetLiquidationRequests;

public class GetLiquidationRequestsQueryResult
{
    public IEnumerable<LiquidationRequestDto> Items { get; set; } = new List<LiquidationRequestDto>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
