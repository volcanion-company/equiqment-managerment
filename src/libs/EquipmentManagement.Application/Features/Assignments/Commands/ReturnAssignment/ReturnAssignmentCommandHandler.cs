using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Enums;
using EquipmentManagement.Domain.Repositories;
using MediatR;

namespace EquipmentManagement.Application.Features.Assignments.Commands.ReturnAssignment;

public class ReturnAssignmentCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ReturnAssignmentCommand, Unit>
{
    public async Task<Unit> Handle(ReturnAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await unitOfWork.Assignments.GetByIdAsync(request.AssignmentId, cancellationToken);
        
        if (assignment == null || assignment.IsDeleted)
        {
            throw new NotFoundException(nameof(Assignment), request.AssignmentId);
        }

        if (assignment.Status == AssignmentStatus.Returned)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "Status", new[] { "Assignment has already been returned" } }
            });
        }

        if (assignment.Status == AssignmentStatus.Lost)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "Status", new[] { "Cannot return a lost assignment" } }
            });
        }

        // Get equipment
        var equipment = await unitOfWork.Equipments.GetByIdAsync(assignment.EquipmentId, cancellationToken);
        
        if (equipment == null || equipment.IsDeleted)
        {
            throw new NotFoundException(nameof(Equipment), assignment.EquipmentId);
        }

        // Update assignment
        assignment.Status = AssignmentStatus.Returned;
        assignment.ReturnDate = DateTime.UtcNow;
        assignment.Notes = string.IsNullOrEmpty(request.ReturnNotes) 
            ? assignment.Notes 
            : $"{assignment.Notes}\n[RETURN] {request.ReturnNotes}";
        assignment.UpdatedAt = DateTime.UtcNow;
        unitOfWork.Assignments.Update(assignment);

        // Update equipment status
        equipment.Status = request.NeedsMaintenance 
            ? EquipmentStatus.Repairing 
            : EquipmentStatus.New;
        equipment.UpdatedAt = DateTime.UtcNow;
        unitOfWork.Equipments.Update(equipment);

        // Import back to warehouse
        var warehouseItem = await unitOfWork.WarehouseItems.GetByEquipmentTypeAsync(equipment.Type, cancellationToken);
        
        if (warehouseItem != null && !warehouseItem.IsDeleted)
        {
            // Increment quantity
            warehouseItem.Quantity += 1;
            warehouseItem.UpdatedAt = DateTime.UtcNow;
            unitOfWork.WarehouseItems.Update(warehouseItem);

            // Create warehouse transaction for import
            var transaction = new WarehouseTransaction
            {
                Id = Guid.NewGuid(),
                WarehouseItemId = warehouseItem.Id,
                Type = WarehouseTransactionType.Import,
                Quantity = 1,
                Reason = $"Return from assignment {assignment.Id} ({assignment.AssignedToUserId ?? assignment.AssignedToDepartment})",
                PerformedBy = request.ReturnedBy ?? "System",
                TransactionDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await unitOfWork.WarehouseTransactions.AddAsync(transaction, cancellationToken);
        }
        else
        {
            // Warehouse item doesn't exist, create new one
            var newWarehouseItem = new WarehouseItem
            {
                Id = Guid.NewGuid(),
                EquipmentType = equipment.Type,
                Quantity = 1,
                MinThreshold = 5,
                Notes = "Auto-created from equipment return",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await unitOfWork.WarehouseItems.AddAsync(newWarehouseItem, cancellationToken);

            // Create transaction
            var transaction = new WarehouseTransaction
            {
                Id = Guid.NewGuid(),
                WarehouseItemId = newWarehouseItem.Id,
                Type = WarehouseTransactionType.Import,
                Quantity = 1,
                Reason = $"Return from assignment {assignment.Id} - Auto-created warehouse item",
                PerformedBy = request.ReturnedBy ?? "System",
                TransactionDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await unitOfWork.WarehouseTransactions.AddAsync(transaction, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
