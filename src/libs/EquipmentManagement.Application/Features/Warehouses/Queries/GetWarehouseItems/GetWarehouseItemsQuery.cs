using EquipmentManagement.Application.Common.Models;
using EquipmentManagement.Application.Features.Warehouses.DTOs;
using MediatR;

namespace EquipmentManagement.Application.Features.Warehouses.Queries.GetWarehouseItems;

public class GetWarehouseItemsQuery : IRequest<PagedResult<WarehouseItemDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? EquipmentType { get; set; }
    public bool? LowStockOnly { get; set; }
}
