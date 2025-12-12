using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Warehouses.Commands.CreateWarehouseItem;

public class CreateWarehouseItemCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateWarehouseItemCommand, Guid>
{
    public async Task<Guid> Handle(CreateWarehouseItemCommand request, CancellationToken cancellationToken)
    {
        // Check if equipment type already exists
        var existingItem = await unitOfWork.WarehouseItems.GetByEquipmentTypeAsync(request.EquipmentType, cancellationToken);
        
        if (existingItem != null && !existingItem.IsDeleted)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "EquipmentType", new[] { "Equipment type already exists in warehouse" } }
            });
        }

        var warehouseItem = request.Adapt<WarehouseItem>();
        warehouseItem.Id = Guid.NewGuid();
        warehouseItem.CreatedAt = DateTime.UtcNow;
        warehouseItem.IsDeleted = false;

        await unitOfWork.WarehouseItems.AddAsync(warehouseItem, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return warehouseItem.Id;
    }
}
