using EquipmentManagement.Application.Features.Maintenances.DTOs;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Maintenances.Queries.GetMaintenanceRequests;

public class GetMaintenanceRequestsQueryHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<GetMaintenanceRequestsQuery, GetMaintenanceRequestsQueryResult>
{
    public async Task<GetMaintenanceRequestsQueryResult> Handle(GetMaintenanceRequestsQuery request, CancellationToken cancellationToken)
    {
        var (maintenanceRequests, totalCount) = await unitOfWork.MaintenanceRequests.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.EquipmentId,
            request.TechnicianId,
            cancellationToken);

        // Filter by status if provided
        var filteredRequests = maintenanceRequests;
        if (request.Status.HasValue)
        {
            filteredRequests = maintenanceRequests.Where(m => (int)m.Status == request.Status.Value);
            totalCount = filteredRequests.Count();
        }

        var dtos = filteredRequests.Adapt<IEnumerable<MaintenanceRequestDto>>();

        return new GetMaintenanceRequestsQueryResult
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
