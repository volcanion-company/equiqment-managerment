using EquipmentManagement.Application.Features.Liquidations.DTOs;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Liquidations.Queries.GetLiquidationRequests;

public class GetLiquidationRequestsQueryHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<GetLiquidationRequestsQuery, GetLiquidationRequestsQueryResult>
{
    public async Task<GetLiquidationRequestsQueryResult> Handle(
        GetLiquidationRequestsQuery request, 
        CancellationToken cancellationToken)
    {
        var (liquidationRequests, totalCount) = await unitOfWork.LiquidationRequests.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.IsApproved,
            cancellationToken);

        var dtos = liquidationRequests.Adapt<IEnumerable<LiquidationRequestDto>>();

        return new GetLiquidationRequestsQueryResult
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
