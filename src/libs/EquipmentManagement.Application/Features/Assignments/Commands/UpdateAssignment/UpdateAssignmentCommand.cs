using MediatR;

namespace EquipmentManagement.Application.Features.Assignments.Commands.UpdateAssignment;

/// <summary>
/// Command to update assignment information
/// </summary>
public class UpdateAssignmentCommand : IRequest<Unit>
{
    /// <summary>
    /// Assignment ID to update
    /// </summary>
    public Guid AssignmentId { get; set; }

    /// <summary>
    /// Updated assigned date
    /// </summary>
    public DateTime? AssignedDate { get; set; }

    /// <summary>
    /// Updated notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Updated assigned to user ID
    /// </summary>
    public string? AssignedToUserId { get; set; }

    /// <summary>
    /// Updated assigned to department
    /// </summary>
    public string? AssignedToDepartment { get; set; }
}
