using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Enums;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Warehouses.Commands.CreateTransaction;

public class CreateWarehouseTransactionCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateWarehouseTransactionCommand, Guid>
{
    public async Task<Guid> Handle(CreateWarehouseTransactionCommand request, CancellationToken cancellationToken)
    {
        var warehouseItem = await unitOfWork.WarehouseItems.GetByIdAsync(request.WarehouseItemId, cancellationToken);
        
        if (warehouseItem == null || warehouseItem.IsDeleted)
        {
            throw new NotFoundException(nameof(WarehouseItem), request.WarehouseItemId);
        }

        // Update quantity
        if (request.Type == WarehouseTransactionType.Import)
        {
            warehouseItem.Quantity += request.Quantity;
        }
        else if (request.Type == WarehouseTransactionType.Export)
        {
            if (warehouseItem.Quantity < request.Quantity)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Quantity", new[] { "Insufficient quantity in warehouse" } }
                });
            }
            warehouseItem.Quantity -= request.Quantity;
        }

        warehouseItem.UpdatedAt = DateTime.UtcNow;
        unitOfWork.WarehouseItems.Update(warehouseItem);

        var transaction = request.Adapt<WarehouseTransaction>();
        transaction.Id = Guid.NewGuid();
        transaction.TransactionDate = DateTime.UtcNow;
        transaction.CreatedAt = DateTime.UtcNow;
        transaction.IsDeleted = false;

        await unitOfWork.WarehouseTransactions.AddAsync(transaction, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return transaction.Id;
    }
}
