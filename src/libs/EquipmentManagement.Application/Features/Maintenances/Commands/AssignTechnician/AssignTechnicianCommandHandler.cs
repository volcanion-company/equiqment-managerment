using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Enums;
using EquipmentManagement.Domain.Repositories;
using MediatR;

namespace EquipmentManagement.Application.Features.Maintenances.Commands.AssignTechnician;

public class AssignTechnicianCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AssignTechnicianCommand, Unit>
{
    public async Task<Unit> Handle(AssignTechnicianCommand request, CancellationToken cancellationToken)
    {
        var maintenanceRequest = await unitOfWork.MaintenanceRequests.GetByIdAsync(request.MaintenanceRequestId, cancellationToken);
        
        if (maintenanceRequest == null || maintenanceRequest.IsDeleted)
        {
            throw new NotFoundException(nameof(MaintenanceRequest), request.MaintenanceRequestId);
        }

        // Only allow assigning to pending requests
        if (maintenanceRequest.Status != MaintenanceStatus.Pending)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "Status", new[] { $"Can only assign technician to pending maintenance requests. Current status: {maintenanceRequest.Status}" } }
            });
        }

        // Update technician assignment
        maintenanceRequest.TechnicianId = request.TechnicianId;
        
        if (!string.IsNullOrEmpty(request.AssignmentNotes))
        {
            maintenanceRequest.Notes = string.IsNullOrEmpty(maintenanceRequest.Notes) 
                ? $"[ASSIGNED] {request.AssignmentNotes}" 
                : $"{maintenanceRequest.Notes}\n[ASSIGNED] {request.AssignmentNotes}";
        }
        
        maintenanceRequest.UpdatedAt = DateTime.UtcNow;
        unitOfWork.MaintenanceRequests.Update(maintenanceRequest);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
