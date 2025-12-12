using FluentValidation;

namespace EquipmentManagement.Application.Features.Liquidations.Commands.RejectLiquidation;

public class RejectLiquidationCommandValidator : AbstractValidator<RejectLiquidationCommand>
{
    public RejectLiquidationCommandValidator()
    {
        RuleFor(x => x.LiquidationRequestId)
            .NotEmpty()
            .WithMessage("Liquidation request ID is required");

        RuleFor(x => x.RejectedBy)
            .NotEmpty()
            .WithMessage("Reviewer ID is required")
            .MaximumLength(100)
            .WithMessage("Reviewer ID cannot exceed 100 characters");

        RuleFor(x => x.RejectionReason)
            .NotEmpty()
            .WithMessage("Rejection reason is required")
            .MaximumLength(500)
            .WithMessage("Rejection reason cannot exceed 500 characters");
    }
}
