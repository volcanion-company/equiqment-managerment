using FluentValidation;

namespace EquipmentManagement.Application.Features.Maintenances.Queries.GetMaintenanceRequests;

public class GetMaintenanceRequestsQueryValidator : AbstractValidator<GetMaintenanceRequestsQuery>
{
    public GetMaintenanceRequestsQueryValidator()
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
            .InclusiveBetween(1, 4)
            .When(x => x.Status.HasValue)
            .WithMessage("Status must be 1 (Pending), 2 (InProgress), 3 (Completed), or 4 (Cancelled)");
    }
}
