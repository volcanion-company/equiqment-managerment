using FluentValidation;

namespace EquipmentManagement.Application.Features.Audits.Queries.GetAuditsForSync;

public class GetAuditsForSyncQueryValidator : AbstractValidator<GetAuditsForSyncQuery>
{
    public GetAuditsForSyncQueryValidator()
    {
        RuleFor(x => x.SinceDate)
            .LessThanOrEqualTo(DateTime.UtcNow).When(x => x.SinceDate.HasValue)
            .WithMessage("Sync date cannot be in the future");
    }
}
