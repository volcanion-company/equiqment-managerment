using FluentValidation;

namespace EquipmentManagement.Application.Features.Maintenances.Queries.GetMaintenancesByTechnician;

public class GetMaintenancesByTechnicianQueryValidator : AbstractValidator<GetMaintenancesByTechnicianQuery>
{
    public GetMaintenancesByTechnicianQueryValidator()
    {
        RuleFor(x => x.TechnicianId)
            .NotEmpty()
            .WithMessage("Technician ID is required")
            .MaximumLength(100)
            .WithMessage("Technician ID cannot exceed 100 characters");
    }
}
