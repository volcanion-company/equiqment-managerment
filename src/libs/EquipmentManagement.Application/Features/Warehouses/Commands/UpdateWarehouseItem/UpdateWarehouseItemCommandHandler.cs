using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Repositories;
using MediatR;

namespace EquipmentManagement.Application.Features.Warehouses.Commands.UpdateWarehouseItem;

public class UpdateWarehouseItemCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateWarehouseItemCommand, Unit>
{
    public async Task<Unit> Handle(UpdateWarehouseItemCommand request, CancellationToken cancellationToken)
    {
        var warehouseItem = await unitOfWork.WarehouseItems.GetByIdAsync(request.Id, cancellationToken);

        if (warehouseItem == null || warehouseItem.IsDeleted)
        {
            throw new NotFoundException(nameof(WarehouseItem), request.Id);
        }

        // Check if changing to a different equipment type that already exists
        if (warehouseItem.EquipmentType != request.EquipmentType)
        {
            var existingItem = await unitOfWork.WarehouseItems.GetByEquipmentTypeAsync(request.EquipmentType, cancellationToken);
            
            if (existingItem != null && !existingItem.IsDeleted && existingItem.Id != request.Id)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "EquipmentType", new[] { "Equipment type already exists in warehouse" } }
                });
            }
        }

        warehouseItem.EquipmentType = request.EquipmentType;
        warehouseItem.Quantity = request.Quantity;
        warehouseItem.MinThreshold = request.MinThreshold;
        warehouseItem.Notes = request.Notes;
        warehouseItem.UpdatedAt = DateTime.UtcNow;

        unitOfWork.WarehouseItems.Update(warehouseItem);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
