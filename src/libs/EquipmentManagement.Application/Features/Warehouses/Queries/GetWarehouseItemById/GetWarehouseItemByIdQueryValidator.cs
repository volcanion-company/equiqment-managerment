using FluentValidation;

namespace EquipmentManagement.Application.Features.Warehouses.Queries.GetWarehouseItemById;

public class GetWarehouseItemByIdQueryValidator : AbstractValidator<GetWarehouseItemByIdQuery>
{
    public GetWarehouseItemByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");
    }
}
