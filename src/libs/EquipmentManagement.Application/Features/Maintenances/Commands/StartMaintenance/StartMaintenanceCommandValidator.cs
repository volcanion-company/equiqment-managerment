using FluentValidation;

namespace EquipmentManagement.Application.Features.Maintenances.Commands.StartMaintenance;

public class StartMaintenanceCommandValidator : AbstractValidator<StartMaintenanceCommand>
{
    public StartMaintenanceCommandValidator()
    {
        RuleFor(x => x.MaintenanceRequestId)
            .NotEmpty()
            .WithMessage("Maintenance request ID is required");

        RuleFor(x => x.TechnicianId)
            .NotEmpty()
            .WithMessage("Technician ID is required")
            .MaximumLength(100)
            .WithMessage("Technician ID cannot exceed 100 characters");

        RuleFor(x => x.StartNotes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.StartNotes))
            .WithMessage("Start notes cannot exceed 500 characters");
    }
}
