using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Enums;
using EquipmentManagement.Domain.Repositories;
using MediatR;

namespace EquipmentManagement.Application.Features.Maintenances.Commands.CompleteMaintenance;

public class CompleteMaintenanceCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CompleteMaintenanceCommand, Unit>
{
    public async Task<Unit> Handle(CompleteMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var maintenanceRequest = await unitOfWork.MaintenanceRequests.GetByIdAsync(request.MaintenanceRequestId, cancellationToken);
        
        if (maintenanceRequest == null || maintenanceRequest.IsDeleted)
        {
            throw new NotFoundException(nameof(MaintenanceRequest), request.MaintenanceRequestId);
        }

        // Must be in InProgress status
        if (maintenanceRequest.Status != MaintenanceStatus.InProgress)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "Status", new[] { $"Can only complete in-progress maintenance requests. Current status: {maintenanceRequest.Status}" } }
            });
        }

        // Verify the technician is the assigned one
        if (maintenanceRequest.TechnicianId != request.TechnicianId)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "TechnicianId", new[] { $"Only the assigned technician ({maintenanceRequest.TechnicianId}) can complete this maintenance" } }
            });
        }

        // Update maintenance request
        maintenanceRequest.Status = MaintenanceStatus.Completed;
        maintenanceRequest.EndDate = DateTime.UtcNow;
        maintenanceRequest.Cost = request.Cost;
        
        if (!string.IsNullOrEmpty(request.CompletionNotes))
        {
            maintenanceRequest.Notes = string.IsNullOrEmpty(maintenanceRequest.Notes) 
                ? $"[COMPLETED] {request.CompletionNotes}" 
                : $"{maintenanceRequest.Notes}\n[COMPLETED] {request.CompletionNotes}";
        }
        
        maintenanceRequest.UpdatedAt = DateTime.UtcNow;
        unitOfWork.MaintenanceRequests.Update(maintenanceRequest);

        // Update equipment status
        var equipment = await unitOfWork.Equipments.GetByIdAsync(maintenanceRequest.EquipmentId, cancellationToken);
        if (equipment != null && !equipment.IsDeleted)
        {
            equipment.Status = request.StillNeedsMaintenance 
                ? EquipmentStatus.Repairing 
                : EquipmentStatus.New;
            equipment.UpdatedAt = DateTime.UtcNow;
            unitOfWork.Equipments.Update(equipment);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
