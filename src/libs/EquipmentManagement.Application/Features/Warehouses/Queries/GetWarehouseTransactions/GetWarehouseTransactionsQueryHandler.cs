using EquipmentManagement.Application.Common.Models;
using EquipmentManagement.Application.Features.Warehouses.DTOs;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Warehouses.Queries.GetWarehouseTransactions;

public class GetWarehouseTransactionsQueryHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<GetWarehouseTransactionsQuery, PagedResult<WarehouseTransactionDto>>
{
    public async Task<PagedResult<WarehouseTransactionDto>> Handle(GetWarehouseTransactionsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await unitOfWork.WarehouseTransactions.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.WarehouseItemId,
            cancellationToken);

        var transactionDtos = items
            .Where(t => !t.IsDeleted)
            .Adapt<List<WarehouseTransactionDto>>();

        return new PagedResult<WarehouseTransactionDto>
        {
            Items = transactionDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
