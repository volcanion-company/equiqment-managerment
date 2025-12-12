using FluentValidation;

namespace EquipmentManagement.Application.Features.Assignments.Commands.DeleteAssignment;

public class DeleteAssignmentCommandValidator : AbstractValidator<DeleteAssignmentCommand>
{
    public DeleteAssignmentCommandValidator()
    {
        RuleFor(x => x.AssignmentId)
            .NotEmpty()
            .WithMessage("Assignment ID is required");
    }
}
