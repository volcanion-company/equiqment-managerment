using MediatR;

namespace EquipmentManagement.Application.Features.Assignments.Commands.CreateAssignment;

public class CreateAssignmentCommand : IRequest<Guid>
{
    public Guid EquipmentId { get; set; }
    public string? AssignedToUserId { get; set; }
    public string? AssignedToDepartment { get; set; }
    public DateTime AssignedDate { get; set; }
    public string? Notes { get; set; }
    public string? AssignedBy { get; set; }
}
