using FluentValidation;

namespace EquipmentManagement.Application.Features.Maintenances.Commands.CompleteMaintenance;

public class CompleteMaintenanceCommandValidator : AbstractValidator<CompleteMaintenanceCommand>
{
    public CompleteMaintenanceCommandValidator()
    {
        RuleFor(x => x.MaintenanceRequestId)
            .NotEmpty()
            .WithMessage("Maintenance request ID is required");

        RuleFor(x => x.TechnicianId)
            .NotEmpty()
            .WithMessage("Technician ID is required")
            .MaximumLength(100)
            .WithMessage("Technician ID cannot exceed 100 characters");

        RuleFor(x => x.Cost)
            .GreaterThan(0)
            .WithMessage("Cost must be greater than 0");

        RuleFor(x => x.CompletionNotes)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.CompletionNotes))
            .WithMessage("Completion notes cannot exceed 1000 characters");
    }
}
