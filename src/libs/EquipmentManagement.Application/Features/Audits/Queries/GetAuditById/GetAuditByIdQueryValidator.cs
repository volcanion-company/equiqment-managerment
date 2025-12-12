using FluentValidation;

namespace EquipmentManagement.Application.Features.Audits.Queries.GetAuditById;

public class GetAuditByIdQueryValidator : AbstractValidator<GetAuditByIdQuery>
{
    public GetAuditByIdQueryValidator()
    {
        RuleFor(x => x.AuditRecordId)
            .NotEmpty().WithMessage("Audit record ID is required");
    }
}
