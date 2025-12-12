using FluentValidation;

namespace EquipmentManagement.Application.Features.Assignments.Queries.GetAssignmentById;

public class GetAssignmentByIdQueryValidator : AbstractValidator<GetAssignmentByIdQuery>
{
    public GetAssignmentByIdQueryValidator()
    {
        RuleFor(x => x.AssignmentId)
            .NotEmpty()
            .WithMessage("Assignment ID is required");
    }
}
