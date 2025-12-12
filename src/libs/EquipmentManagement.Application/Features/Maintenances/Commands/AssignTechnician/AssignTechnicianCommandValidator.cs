using FluentValidation;

namespace EquipmentManagement.Application.Features.Maintenances.Commands.AssignTechnician;

public class AssignTechnicianCommandValidator : AbstractValidator<AssignTechnicianCommand>
{
    public AssignTechnicianCommandValidator()
    {
        RuleFor(x => x.MaintenanceRequestId)
            .NotEmpty()
            .WithMessage("Maintenance request ID is required");

        RuleFor(x => x.TechnicianId)
            .NotEmpty()
            .WithMessage("Technician ID is required")
            .MaximumLength(100)
            .WithMessage("Technician ID cannot exceed 100 characters");

        RuleFor(x => x.AssignmentNotes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.AssignmentNotes))
            .WithMessage("Assignment notes cannot exceed 500 characters");
    }
}
