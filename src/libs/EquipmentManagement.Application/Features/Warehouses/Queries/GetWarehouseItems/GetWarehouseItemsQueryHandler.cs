using EquipmentManagement.Application.Common.Models;
using EquipmentManagement.Application.Features.Warehouses.DTOs;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Warehouses.Queries.GetWarehouseItems;

public class GetWarehouseItemsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetWarehouseItemsQuery, PagedResult<WarehouseItemDto>>
{
    public async Task<PagedResult<WarehouseItemDto>> Handle(GetWarehouseItemsQuery request, CancellationToken cancellationToken)
    {
        // Get all warehouse items
        var allItems = await unitOfWork.WarehouseItems.GetAllAsync(cancellationToken);

        // Filter out deleted items
        var items = allItems.Where(w => !w.IsDeleted);

        // Filter by equipment type
        if (!string.IsNullOrWhiteSpace(request.EquipmentType))
        {
            items = items.Where(w => w.EquipmentType.Contains(request.EquipmentType, StringComparison.OrdinalIgnoreCase));
        }

        // Filter low stock items only
        if (request.LowStockOnly == true)
        {
            items = items.Where(w => w.Quantity <= w.MinThreshold);
        }

        var itemsList = items.OrderBy(w => w.EquipmentType).ToList();
        var totalCount = itemsList.Count;

        var pagedItems = itemsList
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var itemDtos = pagedItems.Adapt<List<WarehouseItemDto>>();

        return new PagedResult<WarehouseItemDto>
        {
            Items = itemDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
