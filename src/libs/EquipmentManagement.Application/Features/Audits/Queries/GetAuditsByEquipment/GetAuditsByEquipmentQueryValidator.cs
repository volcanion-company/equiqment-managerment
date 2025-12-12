using FluentValidation;

namespace EquipmentManagement.Application.Features.Audits.Queries.GetAuditsByEquipment;

public class GetAuditsByEquipmentQueryValidator : AbstractValidator<GetAuditsByEquipmentQuery>
{
    public GetAuditsByEquipmentQueryValidator()
    {
        RuleFor(x => x.EquipmentId)
            .NotEmpty().WithMessage("Equipment ID is required");
    }
}
