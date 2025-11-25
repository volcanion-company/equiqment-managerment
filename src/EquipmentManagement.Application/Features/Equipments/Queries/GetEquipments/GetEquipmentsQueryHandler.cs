using EquipmentManagement.Application.Common.Interfaces;
using EquipmentManagement.Application.Common.Models;
using EquipmentManagement.Application.Features.Equipments.DTOs;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Equipments.Queries.GetEquipments;

public class GetEquipmentsQueryHandler : IRequestHandler<GetEquipmentsQuery, PagedResult<EquipmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public GetEquipmentsQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<PagedResult<EquipmentDto>> Handle(GetEquipmentsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"equipments_{request.PageNumber}_{request.PageSize}_{request.Type}_{request.Status}_{request.Keyword}";
        
        var cachedResult = await _cacheService.GetAsync<PagedResult<EquipmentDto>>(cacheKey, cancellationToken);
        if (cachedResult != null)
        {
            return cachedResult;
        }

        var (items, totalCount) = await _unitOfWork.Equipments.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.Type,
            request.Status,
            request.Keyword,
            cancellationToken);

        var result = new PagedResult<EquipmentDto>
        {
            Items = items.Adapt<IEnumerable<EquipmentDto>>(),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30), cancellationToken);

        return result;
    }
}
