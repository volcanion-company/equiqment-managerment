using EquipmentManagement.Application.Features.Maintenances.DTOs;
using EquipmentManagement.Domain.Enums;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Maintenances.Queries.GetMaintenancesByTechnician;

public class GetMaintenancesByTechnicianQueryHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<GetMaintenancesByTechnicianQuery, IEnumerable<MaintenanceRequestDto>>
{
    public async Task<IEnumerable<MaintenanceRequestDto>> Handle(GetMaintenancesByTechnicianQuery request, CancellationToken cancellationToken)
    {
        var maintenanceRequests = await unitOfWork.MaintenanceRequests.GetByTechnicianIdAsync(request.TechnicianId, cancellationToken);

        if (request.ActiveOnly)
        {
            maintenanceRequests = maintenanceRequests
                .Where(m => m.Status == MaintenanceStatus.Pending || m.Status == MaintenanceStatus.InProgress)
                .OrderBy(m => m.RequestDate);
        }
        else
        {
            maintenanceRequests = maintenanceRequests.OrderByDescending(m => m.RequestDate);
        }

        return maintenanceRequests.Adapt<IEnumerable<MaintenanceRequestDto>>();
    }
}
