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

        // Update quantity based on transaction type
        switch (request.Type)
        {
            case WarehouseTransactionType.Import:
                warehouseItem.Quantity += request.Quantity;
                break;

            case WarehouseTransactionType.Export:
                if (warehouseItem.Quantity < request.Quantity)
                {
                    throw new ValidationException(new Dictionary<string, string[]>
                    {
                        { "Quantity", new[] { $"Insufficient quantity in warehouse. Available: {warehouseItem.Quantity}, Requested: {request.Quantity}" } }
                    });
                }
                warehouseItem.Quantity -= request.Quantity;
                break;

            case WarehouseTransactionType.Adjustment:
                // For adjustment, the quantity represents the new total, not a delta
                // But we still record the delta in the transaction
                var delta = request.Quantity - warehouseItem.Quantity;
                warehouseItem.Quantity = request.Quantity;
                // Override the transaction quantity to record the delta
                request = new CreateWarehouseTransactionCommand
                {
                    WarehouseItemId = request.WarehouseItemId,
                    Type = request.Type,
                    Quantity = delta,
                    Reason = request.Reason,
                    PerformedBy = request.PerformedBy
                };
                break;

            default:
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Type", new[] { "Invalid transaction type" } }
                });
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
