using EquipmentManagement.Application.Features.Liquidations.DTOs;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Liquidations.Queries.GetLiquidationById;

public class GetLiquidationByIdQueryHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<GetLiquidationByIdQuery, LiquidationRequestDto>
{
    public async Task<LiquidationRequestDto> Handle(
        GetLiquidationByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var liquidationRequest = await unitOfWork.LiquidationRequests.GetByIdAsync(
            request.LiquidationRequestId, cancellationToken)
            ?? throw new KeyNotFoundException($"Liquidation request with ID {request.LiquidationRequestId} not found");

        return liquidationRequest.Adapt<LiquidationRequestDto>();
    }
}
