using EquipmentManagement.Application.Features.Warehouses.DTOs;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Warehouses.Queries.GetLowStockItems;

public class GetLowStockItemsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetLowStockItemsQuery, List<WarehouseItemDto>>
{
    public async Task<List<WarehouseItemDto>> Handle(GetLowStockItemsQuery request, CancellationToken cancellationToken)
    {
        var lowStockItems = await unitOfWork.WarehouseItems.GetLowStockItemsAsync(cancellationToken);

        var itemDtos = lowStockItems
            .Where(item => !item.IsDeleted)
            .Adapt<List<WarehouseItemDto>>();

        return itemDtos;
    }
}
