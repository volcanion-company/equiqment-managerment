using EquipmentManagement.Application.Features.Warehouses.DTOs;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Warehouses.Queries.GetWarehouseItemById;

public class GetWarehouseItemByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetWarehouseItemByIdQuery, WarehouseItemDto?>
{
    public async Task<WarehouseItemDto?> Handle(GetWarehouseItemByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await unitOfWork.WarehouseItems.GetByIdAsync(request.Id, cancellationToken);

        if (item == null || item.IsDeleted)
        {
            return null;
        }

        return item.Adapt<WarehouseItemDto>();
    }
}
