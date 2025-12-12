using FluentValidation;

namespace EquipmentManagement.Application.Features.Liquidations.Commands.ApproveLiquidation;

public class ApproveLiquidationCommandValidator : AbstractValidator<ApproveLiquidationCommand>
{
    public ApproveLiquidationCommandValidator()
    {
        RuleFor(x => x.LiquidationRequestId)
            .NotEmpty()
            .WithMessage("Liquidation request ID is required");

        RuleFor(x => x.ApprovedBy)
            .NotEmpty()
            .WithMessage("Approver ID is required")
            .MaximumLength(100)
            .WithMessage("Approver ID cannot exceed 100 characters");

        RuleFor(x => x.LiquidationValue)
            .GreaterThan(0)
            .WithMessage("Liquidation value must be greater than 0");

        RuleFor(x => x.ApprovalNotes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.ApprovalNotes))
            .WithMessage("Approval notes cannot exceed 500 characters");
    }
}
