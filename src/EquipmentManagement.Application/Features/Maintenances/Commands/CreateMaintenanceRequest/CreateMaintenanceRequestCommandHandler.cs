using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Enums;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Maintenances.Commands.CreateMaintenanceRequest;

public class CreateMaintenanceRequestCommandHandler : IRequestHandler<CreateMaintenanceRequestCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateMaintenanceRequestCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateMaintenanceRequestCommand request, CancellationToken cancellationToken)
    {
        var equipment = await _unitOfWork.Equipments.GetByIdAsync(request.EquipmentId, cancellationToken);
        
        if (equipment == null || equipment.IsDeleted)
        {
            throw new NotFoundException(nameof(Equipment), request.EquipmentId);
        }

        // Update equipment status
        equipment.Status = EquipmentStatus.Repairing;
        equipment.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Equipments.Update(equipment);

        var maintenanceRequest = request.Adapt<MaintenanceRequest>();
        maintenanceRequest.Id = Guid.NewGuid();
        maintenanceRequest.Status = MaintenanceStatus.Pending;
        maintenanceRequest.RequestDate = DateTime.UtcNow;
        maintenanceRequest.CreatedAt = DateTime.UtcNow;
        maintenanceRequest.IsDeleted = false;

        await _unitOfWork.MaintenanceRequests.AddAsync(maintenanceRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return maintenanceRequest.Id;
    }
}
