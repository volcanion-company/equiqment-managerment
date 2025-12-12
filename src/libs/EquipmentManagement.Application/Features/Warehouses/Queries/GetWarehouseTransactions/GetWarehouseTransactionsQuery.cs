using EquipmentManagement.Application.Common.Models;
using EquipmentManagement.Application.Features.Warehouses.DTOs;
using MediatR;

namespace EquipmentManagement.Application.Features.Warehouses.Queries.GetWarehouseTransactions;

public class GetWarehouseTransactionsQuery : IRequest<PagedResult<WarehouseTransactionDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? WarehouseItemId { get; set; }
}
