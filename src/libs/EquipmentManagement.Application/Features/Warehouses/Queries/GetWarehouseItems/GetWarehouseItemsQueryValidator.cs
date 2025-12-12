using FluentValidation;

namespace EquipmentManagement.Application.Features.Warehouses.Queries.GetWarehouseItems;

public class GetWarehouseItemsQueryValidator : AbstractValidator<GetWarehouseItemsQuery>
{
    public GetWarehouseItemsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber must be at least 1");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("PageSize must not exceed 100");
    }
}
