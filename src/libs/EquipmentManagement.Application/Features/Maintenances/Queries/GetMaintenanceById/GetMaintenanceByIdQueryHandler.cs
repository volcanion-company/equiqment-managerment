using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Application.Features.Maintenances.DTOs;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Maintenances.Queries.GetMaintenanceById;

public class GetMaintenanceByIdQueryHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<GetMaintenanceByIdQuery, MaintenanceRequestDto>
{
    public async Task<MaintenanceRequestDto> Handle(GetMaintenanceByIdQuery request, CancellationToken cancellationToken)
    {
        var maintenanceRequest = await unitOfWork.MaintenanceRequests.GetByIdAsync(request.MaintenanceRequestId, cancellationToken);
        
        if (maintenanceRequest == null || maintenanceRequest.IsDeleted)
        {
            throw new NotFoundException(nameof(MaintenanceRequest), request.MaintenanceRequestId);
        }

        return maintenanceRequest.Adapt<MaintenanceRequestDto>();
    }
}
