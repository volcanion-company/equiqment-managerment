using FluentValidation;

namespace EquipmentManagement.Application.Features.Warehouses.Commands.CreateWarehouseItem;

public class CreateWarehouseItemCommandValidator : AbstractValidator<CreateWarehouseItemCommand>
{
    public CreateWarehouseItemCommandValidator()
    {
        RuleFor(x => x.EquipmentType)
            .NotEmpty().WithMessage("EquipmentType is required")
            .MaximumLength(200).WithMessage("EquipmentType must not exceed 200 characters");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity must be greater than or equal to 0");

        RuleFor(x => x.MinThreshold)
            .GreaterThanOrEqualTo(0).WithMessage("MinThreshold must be greater than or equal to 0");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes must not exceed 500 characters");
    }
}
