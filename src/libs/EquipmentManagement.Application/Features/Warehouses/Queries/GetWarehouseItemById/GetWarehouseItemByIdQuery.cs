using EquipmentManagement.Application.Features.Warehouses.DTOs;
using MediatR;

namespace EquipmentManagement.Application.Features.Warehouses.Queries.GetWarehouseItemById;

public class GetWarehouseItemByIdQuery : IRequest<WarehouseItemDto?>
{
    public Guid Id { get; set; }
}
