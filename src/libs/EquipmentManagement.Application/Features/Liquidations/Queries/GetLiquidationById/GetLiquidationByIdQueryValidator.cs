using FluentValidation;

namespace EquipmentManagement.Application.Features.Liquidations.Queries.GetLiquidationById;

public class GetLiquidationByIdQueryValidator : AbstractValidator<GetLiquidationByIdQuery>
{
    public GetLiquidationByIdQueryValidator()
    {
        RuleFor(x => x.LiquidationRequestId)
            .NotEmpty()
            .WithMessage("Liquidation request ID is required");
    }
}
