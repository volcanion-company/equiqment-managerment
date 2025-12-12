using FluentValidation;

namespace EquipmentManagement.Application.Features.Liquidations.Commands.UpdateLiquidationRequest;

public class UpdateLiquidationRequestCommandValidator : AbstractValidator<UpdateLiquidationRequestCommand>
{
    public UpdateLiquidationRequestCommandValidator()
    {
        RuleFor(x => x.LiquidationRequestId)
            .NotEmpty()
            .WithMessage("Liquidation request ID is required");

        RuleFor(x => x.LiquidationValue)
            .GreaterThan(0)
            .When(x => x.LiquidationValue.HasValue)
            .WithMessage("Liquidation value must be greater than 0");

        RuleFor(x => x.Note)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Note))
            .WithMessage("Note cannot exceed 1000 characters");
    }
}
