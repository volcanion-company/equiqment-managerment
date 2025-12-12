using FluentValidation;

namespace EquipmentManagement.Application.Features.Assignments.Queries.GetAssignments;

public class GetAssignmentsQueryValidator : AbstractValidator<GetAssignmentsQuery>
{
    public GetAssignmentsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size cannot exceed 100");

        RuleFor(x => x.Status)
            .InclusiveBetween(1, 3)
            .When(x => x.Status.HasValue)
            .WithMessage("Status must be 1 (Assigned), 2 (Returned), or 3 (Lost)");
    }
}
