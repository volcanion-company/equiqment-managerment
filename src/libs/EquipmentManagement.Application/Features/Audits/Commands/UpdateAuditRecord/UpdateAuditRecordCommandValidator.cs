using FluentValidation;

namespace EquipmentManagement.Application.Features.Audits.Commands.UpdateAuditRecord;

public class UpdateAuditRecordCommandValidator : AbstractValidator<UpdateAuditRecordCommand>
{
    public UpdateAuditRecordCommandValidator()
    {
        RuleFor(x => x.AuditRecordId)
            .NotEmpty().WithMessage("Audit record ID is required");

        RuleFor(x => x.Result)
            .IsInEnum().WithMessage("Invalid audit result value");

        RuleFor(x => x.Location)
            .MaximumLength(200).WithMessage("Location cannot exceed 200 characters");

        RuleFor(x => x.Note)
            .MaximumLength(500).WithMessage("Note cannot exceed 500 characters");
    }
}
