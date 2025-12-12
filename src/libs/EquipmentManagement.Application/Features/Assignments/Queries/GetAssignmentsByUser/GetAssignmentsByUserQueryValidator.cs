using FluentValidation;

namespace EquipmentManagement.Application.Features.Assignments.Queries.GetAssignmentsByUser;

public class GetAssignmentsByUserQueryValidator : AbstractValidator<GetAssignmentsByUserQuery>
{
    public GetAssignmentsByUserQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required")
            .MaximumLength(100)
            .WithMessage("User ID cannot exceed 100 characters");
    }
}
