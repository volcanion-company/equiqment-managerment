using EquipmentManagement.Application.Features.Assignments.DTOs;
using MediatR;

namespace EquipmentManagement.Application.Features.Assignments.Queries.GetAssignments;

/// <summary>
/// Query to get paginated list of assignments with filters
/// </summary>
public class GetAssignmentsQuery : IRequest<GetAssignmentsQueryResult>
{
    /// <summary>
    /// Page number (starts from 1)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Filter by equipment ID
    /// </summary>
    public Guid? EquipmentId { get; set; }

    /// <summary>
    /// Filter by user ID
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Filter by assignment status (1=Assigned, 2=Returned, 3=Lost)
    /// </summary>
    public int? Status { get; set; }
}
