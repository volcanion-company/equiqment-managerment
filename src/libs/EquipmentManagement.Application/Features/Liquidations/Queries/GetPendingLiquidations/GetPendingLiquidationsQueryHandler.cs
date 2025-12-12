using EquipmentManagement.Application.Features.Liquidations.DTOs;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Liquidations.Queries.GetPendingLiquidations;

public class GetPendingLiquidationsQueryHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<GetPendingLiquidationsQuery, IEnumerable<LiquidationRequestDto>>
{
    public async Task<IEnumerable<LiquidationRequestDto>> Handle(
        GetPendingLiquidationsQuery request, 
        CancellationToken cancellationToken)
    {
        var pendingRequests = await unitOfWork.LiquidationRequests.GetPendingRequestsAsync(cancellationToken);

        return pendingRequests.Adapt<IEnumerable<LiquidationRequestDto>>();
    }
}
