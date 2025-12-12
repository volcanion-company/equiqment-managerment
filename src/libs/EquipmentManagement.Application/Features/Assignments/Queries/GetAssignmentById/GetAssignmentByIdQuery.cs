using EquipmentManagement.Application.Features.Assignments.DTOs;
using MediatR;

namespace EquipmentManagement.Application.Features.Assignments.Queries.GetAssignmentById;

/// <summary>
/// Query to get assignment detail by ID
/// </summary>
public class GetAssignmentByIdQuery : IRequest<AssignmentDto>
{
    /// <summary>
    /// Assignment ID
    /// </summary>
    public Guid AssignmentId { get; set; }
}
