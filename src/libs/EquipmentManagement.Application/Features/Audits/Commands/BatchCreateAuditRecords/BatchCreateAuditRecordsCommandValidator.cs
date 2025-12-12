using FluentValidation;

namespace EquipmentManagement.Application.Features.Audits.Commands.BatchCreateAuditRecords;

public class BatchCreateAuditRecordsCommandValidator : AbstractValidator<BatchCreateAuditRecordsCommand>
{
    public BatchCreateAuditRecordsCommandValidator()
    {
        RuleFor(x => x.AuditRecords)
            .NotEmpty().WithMessage("Audit records list cannot be empty")
            .Must(list => list.Count <= 1000).WithMessage("Cannot process more than 1000 audit records at once");

        RuleForEach(x => x.AuditRecords).ChildRules(audit =>
        {
            audit.RuleFor(a => a.EquipmentId)
                .NotEmpty().WithMessage("Equipment ID is required");

            audit.RuleFor(a => a.CheckDate)
                .NotEmpty().WithMessage("Check date is required")
                .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1)).WithMessage("Check date cannot be in the future");

            audit.RuleFor(a => a.Result)
                .IsInEnum().WithMessage("Invalid audit result value");

            audit.RuleFor(a => a.Location)
                .MaximumLength(200).WithMessage("Location cannot exceed 200 characters");

            audit.RuleFor(a => a.Note)
                .MaximumLength(500).WithMessage("Note cannot exceed 500 characters");
        });
    }
}
