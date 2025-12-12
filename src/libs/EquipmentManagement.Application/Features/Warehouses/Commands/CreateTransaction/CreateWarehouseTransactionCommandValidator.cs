using FluentValidation;

namespace EquipmentManagement.Application.Features.Warehouses.Commands.CreateTransaction;

public class CreateWarehouseTransactionCommandValidator : AbstractValidator<CreateWarehouseTransactionCommand>
{
    public CreateWarehouseTransactionCommandValidator()
    {
        RuleFor(x => x.WarehouseItemId)
            .NotEmpty().WithMessage("WarehouseItemId is required");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid transaction type");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.PerformedBy)
            .NotEmpty().WithMessage("PerformedBy is required")
            .MaximumLength(200).WithMessage("PerformedBy must not exceed 200 characters");

        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters");
    }
}
