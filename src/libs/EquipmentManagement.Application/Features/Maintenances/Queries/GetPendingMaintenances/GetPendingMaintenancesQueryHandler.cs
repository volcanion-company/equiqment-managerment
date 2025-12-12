using EquipmentManagement.Application.Features.Maintenances.DTOs;
using EquipmentManagement.Domain.Enums;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Maintenances.Queries.GetPendingMaintenances;

public class GetPendingMaintenancesQueryHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<GetPendingMaintenancesQuery, IEnumerable<MaintenanceRequestDto>>
{
    public async Task<IEnumerable<MaintenanceRequestDto>> Handle(GetPendingMaintenancesQuery request, CancellationToken cancellationToken)
    {
        var allRequests = await unitOfWork.MaintenanceRequests.GetAllAsync(cancellationToken);
        
        var pendingRequests = allRequests
            .Where(m => !m.IsDeleted && m.Status == MaintenanceStatus.Pending)
            .OrderBy(m => m.RequestDate); // Oldest first

        return pendingRequests.Adapt<IEnumerable<MaintenanceRequestDto>>();
    }
}
