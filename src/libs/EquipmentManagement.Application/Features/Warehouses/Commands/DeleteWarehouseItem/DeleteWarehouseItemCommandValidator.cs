using FluentValidation;

namespace EquipmentManagement.Application.Features.Warehouses.Commands.DeleteWarehouseItem;

public class DeleteWarehouseItemCommandValidator : AbstractValidator<DeleteWarehouseItemCommand>
{
    public DeleteWarehouseItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");
    }
}
