using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Enums;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Assignments.Commands.CreateAssignment;

public class CreateAssignmentCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateAssignmentCommand, Guid>
{
    public async Task<Guid> Handle(CreateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var equipment = await unitOfWork.Equipments.GetByIdAsync(request.EquipmentId, cancellationToken);
        
        if (equipment == null || equipment.IsDeleted)
        {
            throw new NotFoundException(nameof(Equipment), request.EquipmentId);
        }

        // Check warehouse stock for this equipment type
        var warehouseItem = await unitOfWork.WarehouseItems.GetByEquipmentTypeAsync(equipment.Type, cancellationToken);
        
        if (warehouseItem != null && !warehouseItem.IsDeleted)
        {
            if (warehouseItem.Quantity < 1)
            {
                // Warehouse stock is insufficient - log warning but proceed
                // You can uncomment below to enforce stock checking:
                // throw new ValidationException(new Dictionary<string, string[]>
                // {
                //     { "Stock", new[] { $"Insufficient stock for equipment type {equipment.Type}. Available: {warehouseItem.Quantity}" } }
                // });
            }
            else
            {
                // Auto export from warehouse
                warehouseItem.Quantity -= 1;
                warehouseItem.UpdatedAt = DateTime.UtcNow;
                unitOfWork.WarehouseItems.Update(warehouseItem);

                // Create warehouse transaction for export
                var transaction = new WarehouseTransaction
                {
                    Id = Guid.NewGuid(),
                    WarehouseItemId = warehouseItem.Id,
                    Type = WarehouseTransactionType.Export,
                    Quantity = 1,
                    Reason = $"Auto-export for assignment to {request.AssignedToUserId ?? request.AssignedToDepartment}",
                    PerformedBy = request.AssignedBy ?? "System",
                    TransactionDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                await unitOfWork.WarehouseTransactions.AddAsync(transaction, cancellationToken);
            }
        }

        // Update equipment status
        equipment.Status = EquipmentStatus.InUse;
        equipment.UpdatedAt = DateTime.UtcNow;
        unitOfWork.Equipments.Update(equipment);

        var assignment = request.Adapt<Assignment>();
        assignment.Id = Guid.NewGuid();
        assignment.Status = AssignmentStatus.Assigned;
        assignment.CreatedAt = DateTime.UtcNow;
        assignment.IsDeleted = false;

        await unitOfWork.Assignments.AddAsync(assignment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return assignment.Id;
    }
}
