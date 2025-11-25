using MediatR;

namespace EquipmentManagement.Application.Features.Liquidations.Commands.CreateLiquidationRequest;

public class CreateLiquidationRequestCommand : IRequest<Guid>
{
    public Guid EquipmentId { get; set; }
    public decimal? LiquidationValue { get; set; }
    public string? Note { get; set; }
}
