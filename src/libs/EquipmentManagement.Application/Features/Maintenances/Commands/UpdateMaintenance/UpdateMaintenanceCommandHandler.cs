using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Enums;
using EquipmentManagement.Domain.Repositories;
using MediatR;

namespace EquipmentManagement.Application.Features.Maintenances.Commands.UpdateMaintenance;

public class UpdateMaintenanceCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateMaintenanceCommand, Unit>
{
    public async Task<Unit> Handle(UpdateMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var maintenanceRequest = await unitOfWork.MaintenanceRequests.GetByIdAsync(request.MaintenanceRequestId, cancellationToken);
        
        if (maintenanceRequest == null || maintenanceRequest.IsDeleted)
        {
            throw new NotFoundException(nameof(MaintenanceRequest), request.MaintenanceRequestId);
        }

        // Cannot update completed or cancelled requests
        if (maintenanceRequest.Status == MaintenanceStatus.Completed || maintenanceRequest.Status == MaintenanceStatus.Cancelled)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "Status", new[] { $"Cannot update {maintenanceRequest.Status.ToString().ToLower()} maintenance requests" } }
            });
        }

        // Update fields
        if (!string.IsNullOrEmpty(request.Description))
        {
            maintenanceRequest.Description = request.Description;
        }

        if (!string.IsNullOrEmpty(request.Notes))
        {
            maintenanceRequest.Notes = request.Notes;
        }

        maintenanceRequest.UpdatedAt = DateTime.UtcNow;
        unitOfWork.MaintenanceRequests.Update(maintenanceRequest);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
