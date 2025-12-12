using FluentValidation;

namespace EquipmentManagement.Application.Features.Warehouses.Queries.GetWarehouseTransactions;

public class GetWarehouseTransactionsQueryValidator : AbstractValidator<GetWarehouseTransactionsQuery>
{
    public GetWarehouseTransactionsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber must be at least 1");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("PageSize must not exceed 100");
    }
}
