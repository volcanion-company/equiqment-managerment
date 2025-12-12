using EquipmentManagement.Domain.Enums;
using EquipmentManagement.Domain.Repositories;
using MediatR;

namespace EquipmentManagement.Application.Features.Liquidations.Commands.ApproveLiquidation;

public class ApproveLiquidationCommandHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<ApproveLiquidationCommand, Unit>
{
    public async Task<Unit> Handle(ApproveLiquidationCommand request, CancellationToken cancellationToken)
    {
        // Get liquidation request
        var liquidationRequest = await unitOfWork.LiquidationRequests.GetByIdAsync(
            request.LiquidationRequestId, cancellationToken)
            ?? throw new KeyNotFoundException($"Liquidation request with ID {request.LiquidationRequestId} not found");

        // Validate: must be pending
        if (liquidationRequest.IsApproved)
            throw new InvalidOperationException("Liquidation request is already approved");

        // Get equipment
        var equipment = await unitOfWork.Equipments.GetByIdAsync(
            liquidationRequest.EquipmentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Equipment with ID {liquidationRequest.EquipmentId} not found");

        // Validate: cannot liquidate assigned equipment
        var activeAssignment = (await unitOfWork.Assignments.GetByEquipmentIdAsync(
            equipment.Id, cancellationToken))
            .FirstOrDefault(a => a.Status == AssignmentStatus.Assigned);

        if (activeAssignment != null)
            throw new InvalidOperationException("Cannot liquidate equipment that is currently assigned");

        // Validate: cannot liquidate equipment in maintenance
        var activeMaintenance = (await unitOfWork.MaintenanceRequests.GetByEquipmentIdAsync(
            equipment.Id, cancellationToken))
            .FirstOrDefault(m => m.Status == MaintenanceStatus.Pending || m.Status == MaintenanceStatus.InProgress);

        if (activeMaintenance != null)
            throw new InvalidOperationException("Cannot liquidate equipment that is in maintenance");

        // Update liquidation request
        liquidationRequest.IsApproved = true;
        liquidationRequest.ApprovedBy = request.ApprovedBy;
        liquidationRequest.ApprovedDate = DateTime.UtcNow;
        liquidationRequest.LiquidationValue = request.LiquidationValue;

        // Append approval note
        if (!string.IsNullOrWhiteSpace(request.ApprovalNotes))
        {
            liquidationRequest.Note = string.IsNullOrWhiteSpace(liquidationRequest.Note)
                ? $"[APPROVED] {request.ApprovalNotes}"
                : $"{liquidationRequest.Note}\n[APPROVED] {request.ApprovalNotes}";
        }
        else
        {
            liquidationRequest.Note = string.IsNullOrWhiteSpace(liquidationRequest.Note)
                ? "[APPROVED]"
                : $"{liquidationRequest.Note}\n[APPROVED]";
        }

        // Update equipment status to Liquidated
        equipment.Status = EquipmentStatus.Liquidated;

        // Export from warehouse if exists
        var warehouseItem = await unitOfWork.WarehouseItems.GetByEquipmentTypeAsync(
            equipment.Type, cancellationToken);

        if (warehouseItem != null && warehouseItem.Quantity > 0)
        {
            // Decrease warehouse quantity
            warehouseItem.Quantity--;

            // Create warehouse transaction record
            var warehouseTransaction = new Domain.Entities.WarehouseTransaction
            {
                WarehouseItemId = warehouseItem.Id,
                Type = WarehouseTransactionType.Export,
                Quantity = 1,
                TransactionDate = DateTime.UtcNow,
                PerformedBy = request.ApprovedBy,
                Reason = $"Exported for liquidation approval - Equipment: {equipment.Name} (Code: {equipment.Code})"
            };

            await unitOfWork.WarehouseTransactions.AddAsync(warehouseTransaction, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
