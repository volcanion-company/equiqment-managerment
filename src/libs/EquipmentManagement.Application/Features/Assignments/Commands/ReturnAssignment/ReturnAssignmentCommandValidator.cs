using FluentValidation;

namespace EquipmentManagement.Application.Features.Assignments.Commands.ReturnAssignment;

public class ReturnAssignmentCommandValidator : AbstractValidator<ReturnAssignmentCommand>
{
    public ReturnAssignmentCommandValidator()
    {
        RuleFor(x => x.AssignmentId)
            .NotEmpty()
            .WithMessage("Assignment ID is required");

        RuleFor(x => x.ReturnedBy)
            .NotEmpty()
            .WithMessage("ReturnedBy is required")
            .MaximumLength(100)
            .WithMessage("ReturnedBy cannot exceed 100 characters");

        RuleFor(x => x.ReturnNotes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.ReturnNotes))
            .WithMessage("Return notes cannot exceed 500 characters");
    }
}
