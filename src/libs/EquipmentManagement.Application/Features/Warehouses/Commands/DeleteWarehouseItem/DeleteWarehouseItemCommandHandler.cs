using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Repositories;
using MediatR;

namespace EquipmentManagement.Application.Features.Warehouses.Commands.DeleteWarehouseItem;

public class DeleteWarehouseItemCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteWarehouseItemCommand, Unit>
{
    public async Task<Unit> Handle(DeleteWarehouseItemCommand request, CancellationToken cancellationToken)
    {
        var warehouseItem = await unitOfWork.WarehouseItems.GetByIdAsync(request.Id, cancellationToken);

        if (warehouseItem == null || warehouseItem.IsDeleted)
        {
            throw new NotFoundException(nameof(WarehouseItem), request.Id);
        }

        // Soft delete
        warehouseItem.IsDeleted = true;
        warehouseItem.UpdatedAt = DateTime.UtcNow;

        unitOfWork.WarehouseItems.Update(warehouseItem);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
