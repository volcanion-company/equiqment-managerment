using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Enums;
using EquipmentManagement.Domain.Repositories;
using MediatR;

namespace EquipmentManagement.Application.Features.Maintenances.Commands.CancelMaintenance;

public class CancelMaintenanceCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CancelMaintenanceCommand, Unit>
{
    public async Task<Unit> Handle(CancelMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var maintenanceRequest = await unitOfWork.MaintenanceRequests.GetByIdAsync(request.MaintenanceRequestId, cancellationToken);
        
        if (maintenanceRequest == null || maintenanceRequest.IsDeleted)
        {
            throw new NotFoundException(nameof(MaintenanceRequest), request.MaintenanceRequestId);
        }

        // Cannot cancel completed maintenance
        if (maintenanceRequest.Status == MaintenanceStatus.Completed)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "Status", new[] { "Cannot cancel completed maintenance requests" } }
            });
        }

        // Cannot cancel already cancelled
        if (maintenanceRequest.Status == MaintenanceStatus.Cancelled)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "Status", new[] { "Maintenance request is already cancelled" } }
            });
        }

        // Update status
        maintenanceRequest.Status = MaintenanceStatus.Cancelled;
        maintenanceRequest.Notes = string.IsNullOrEmpty(maintenanceRequest.Notes) 
            ? $"[CANCELLED] {request.CancellationReason}" 
            : $"{maintenanceRequest.Notes}\n[CANCELLED] {request.CancellationReason}";
        maintenanceRequest.UpdatedAt = DateTime.UtcNow;
        unitOfWork.MaintenanceRequests.Update(maintenanceRequest);

        // If equipment was in Repairing status, set it back to New
        var equipment = await unitOfWork.Equipments.GetByIdAsync(maintenanceRequest.EquipmentId, cancellationToken);
        if (equipment != null && !equipment.IsDeleted && equipment.Status == EquipmentStatus.Repairing)
        {
            equipment.Status = EquipmentStatus.New;
            equipment.UpdatedAt = DateTime.UtcNow;
            unitOfWork.Equipments.Update(equipment);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
