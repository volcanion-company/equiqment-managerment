using FluentValidation;

namespace EquipmentManagement.Application.Features.Maintenances.Commands.UpdateMaintenance;

public class UpdateMaintenanceCommandValidator : AbstractValidator<UpdateMaintenanceCommand>
{
    public UpdateMaintenanceCommandValidator()
    {
        RuleFor(x => x.MaintenanceRequestId)
            .NotEmpty()
            .WithMessage("Maintenance request ID is required");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Notes))
            .WithMessage("Notes cannot exceed 1000 characters");
    }
}
