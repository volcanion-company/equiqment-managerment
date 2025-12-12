using EquipmentManagement.Application.Features.Assignments.DTOs;
using MediatR;

namespace EquipmentManagement.Application.Features.Assignments.Queries.GetAssignmentsByUser;

/// <summary>
/// Query to get active assignments for a specific user
/// </summary>
public class GetAssignmentsByUserQuery : IRequest<IEnumerable<AssignmentDto>>
{
    /// <summary>
    /// User ID to get assignments for
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Include only active (Assigned status) assignments. Default is true.
    /// </summary>
    public bool ActiveOnly { get; set; } = true;
}
