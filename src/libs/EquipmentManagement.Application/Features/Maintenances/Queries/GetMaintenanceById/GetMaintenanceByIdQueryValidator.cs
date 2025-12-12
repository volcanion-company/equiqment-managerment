using FluentValidation;

namespace EquipmentManagement.Application.Features.Maintenances.Queries.GetMaintenanceById;

public class GetMaintenanceByIdQueryValidator : AbstractValidator<GetMaintenanceByIdQuery>
{
    public GetMaintenanceByIdQueryValidator()
    {
        RuleFor(x => x.MaintenanceRequestId)
            .NotEmpty()
            .WithMessage("Maintenance request ID is required");
    }
}
