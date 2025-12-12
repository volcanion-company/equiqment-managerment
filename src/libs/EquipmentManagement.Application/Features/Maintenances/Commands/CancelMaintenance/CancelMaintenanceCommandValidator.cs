using FluentValidation;

namespace EquipmentManagement.Application.Features.Maintenances.Commands.CancelMaintenance;

public class CancelMaintenanceCommandValidator : AbstractValidator<CancelMaintenanceCommand>
{
    public CancelMaintenanceCommandValidator()
    {
        RuleFor(x => x.MaintenanceRequestId)
            .NotEmpty()
            .WithMessage("Maintenance request ID is required");

        RuleFor(x => x.CancellationReason)
            .NotEmpty()
            .WithMessage("Cancellation reason is required")
            .MaximumLength(500)
            .WithMessage("Cancellation reason cannot exceed 500 characters");

        RuleFor(x => x.CancelledBy)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.CancelledBy))
            .WithMessage("CancelledBy cannot exceed 100 characters");
    }
}
