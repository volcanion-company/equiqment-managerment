using EquipmentManagement.Application.Features.Assignments.DTOs;

namespace EquipmentManagement.Application.Features.Assignments.Queries.GetAssignments;

public class GetAssignmentsQueryResult
{
    public IEnumerable<AssignmentDto> Items { get; set; } = new List<AssignmentDto>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
