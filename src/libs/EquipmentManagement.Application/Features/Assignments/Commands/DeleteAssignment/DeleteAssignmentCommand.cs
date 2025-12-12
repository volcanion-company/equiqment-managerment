using MediatR;

namespace EquipmentManagement.Application.Features.Assignments.Commands.DeleteAssignment;

/// <summary>
/// Command to soft delete an assignment
/// </summary>
public class DeleteAssignmentCommand : IRequest<Unit>
{
    /// <summary>
    /// Assignment ID to delete
    /// </summary>
    public Guid AssignmentId { get; set; }
}
