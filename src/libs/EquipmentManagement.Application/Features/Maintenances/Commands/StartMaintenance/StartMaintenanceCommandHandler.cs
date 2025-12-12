using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Enums;
using EquipmentManagement.Domain.Repositories;
using MediatR;

namespace EquipmentManagement.Application.Features.Maintenances.Commands.StartMaintenance;

public class StartMaintenanceCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<StartMaintenanceCommand, Unit>
{
    public async Task<Unit> Handle(StartMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var maintenanceRequest = await unitOfWork.MaintenanceRequests.GetByIdAsync(request.MaintenanceRequestId, cancellationToken);
        
        if (maintenanceRequest == null || maintenanceRequest.IsDeleted)
        {
            throw new NotFoundException(nameof(MaintenanceRequest), request.MaintenanceRequestId);
        }

        // Must be in Pending status
        if (maintenanceRequest.Status != MaintenanceStatus.Pending)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "Status", new[] { $"Can only start pending maintenance requests. Current status: {maintenanceRequest.Status}" } }
            });
        }

        // Must have assigned technician
        if (string.IsNullOrEmpty(maintenanceRequest.TechnicianId))
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "TechnicianId", new[] { "Maintenance request must have an assigned technician before starting" } }
            });
        }

        // Verify the technician is the assigned one
        if (maintenanceRequest.TechnicianId != request.TechnicianId)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "TechnicianId", new[] { $"Only the assigned technician ({maintenanceRequest.TechnicianId}) can start this maintenance" } }
            });
        }

        // Update status and start date
        maintenanceRequest.Status = MaintenanceStatus.InProgress;
        maintenanceRequest.StartDate = DateTime.UtcNow;
        
        if (!string.IsNullOrEmpty(request.StartNotes))
        {
            maintenanceRequest.Notes = string.IsNullOrEmpty(maintenanceRequest.Notes) 
                ? $"[STARTED] {request.StartNotes}" 
                : $"{maintenanceRequest.Notes}\n[STARTED] {request.StartNotes}";
        }
        
        maintenanceRequest.UpdatedAt = DateTime.UtcNow;
        unitOfWork.MaintenanceRequests.Update(maintenanceRequest);

        // Update equipment status to Repairing
        var equipment = await unitOfWork.Equipments.GetByIdAsync(maintenanceRequest.EquipmentId, cancellationToken);
        if (equipment != null && !equipment.IsDeleted)
        {
            equipment.Status = EquipmentStatus.Repairing;
            equipment.UpdatedAt = DateTime.UtcNow;
            unitOfWork.Equipments.Update(equipment);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
