using FluentValidation;

namespace EquipmentManagement.Application.Features.Warehouses.Commands.CreateTransaction;

public class CreateWarehouseTransactionCommandValidator : AbstractValidator<CreateWarehouseTransactionCommand>
{
    public CreateWarehouseTransactionCommandValidator()
    {
        RuleFor(x => x.WarehouseItemId)
            .NotEmpty().WithMessage("WarehouseItemId is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");
    }
}
