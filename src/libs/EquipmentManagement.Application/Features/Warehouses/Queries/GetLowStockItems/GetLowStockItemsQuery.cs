using EquipmentManagement.Application.Features.Warehouses.DTOs;
using MediatR;

namespace EquipmentManagement.Application.Features.Warehouses.Queries.GetLowStockItems;

public class GetLowStockItemsQuery : IRequest<List<WarehouseItemDto>>
{
}
