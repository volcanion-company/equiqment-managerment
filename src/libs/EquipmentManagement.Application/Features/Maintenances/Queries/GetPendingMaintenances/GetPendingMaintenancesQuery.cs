using EquipmentManagement.Application.Features.Maintenances.DTOs;
using MediatR;

namespace EquipmentManagement.Application.Features.Maintenances.Queries.GetPendingMaintenances;

/// <summary>
/// Query to get pending maintenance requests
/// </summary>
public class GetPendingMaintenancesQuery : IRequest<IEnumerable<MaintenanceRequestDto>>
{
}
