using EquipmentManagement.Application.Features.Maintenances.DTOs;

namespace EquipmentManagement.Application.Features.Maintenances.Queries.GetMaintenanceRequests;

public class GetMaintenanceRequestsQueryResult
{
    public IEnumerable<MaintenanceRequestDto> Items { get; set; } = new List<MaintenanceRequestDto>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
