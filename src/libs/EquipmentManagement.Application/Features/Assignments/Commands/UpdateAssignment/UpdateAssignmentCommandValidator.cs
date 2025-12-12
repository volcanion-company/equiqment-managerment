using FluentValidation;

namespace EquipmentManagement.Application.Features.Assignments.Commands.UpdateAssignment;

public class UpdateAssignmentCommandValidator : AbstractValidator<UpdateAssignmentCommand>
{
    public UpdateAssignmentCommandValidator()
    {
        RuleFor(x => x.AssignmentId)
            .NotEmpty()
            .WithMessage("Assignment ID is required");

        RuleFor(x => x.AssignedDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.AssignedDate.HasValue)
            .WithMessage("Assigned date cannot be in the future");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Notes))
            .WithMessage("Notes cannot exceed 500 characters");

        RuleFor(x => x.AssignedToUserId)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.AssignedToUserId))
            .WithMessage("User ID cannot exceed 100 characters");

        RuleFor(x => x.AssignedToDepartment)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.AssignedToDepartment))
            .WithMessage("Department cannot exceed 200 characters");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.AssignedToUserId) || !string.IsNullOrEmpty(x.AssignedToDepartment))
            .When(x => x.AssignedToUserId != null || x.AssignedToDepartment != null)
            .WithMessage("Either AssignedToUserId or AssignedToDepartment must be provided");
    }
}
