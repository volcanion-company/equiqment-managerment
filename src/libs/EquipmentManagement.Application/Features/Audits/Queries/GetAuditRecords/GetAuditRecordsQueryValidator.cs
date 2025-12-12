using FluentValidation;

namespace EquipmentManagement.Application.Features.Audits.Queries.GetAuditRecords;

public class GetAuditRecordsQueryValidator : AbstractValidator<GetAuditRecordsQuery>
{
    public GetAuditRecordsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("Page size must be at least 1")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100");

        RuleFor(x => x.Result)
            .IsInEnum().When(x => x.Result.HasValue).WithMessage("Invalid audit result value");

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate).When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("FromDate must be less than or equal to ToDate");
    }
}
