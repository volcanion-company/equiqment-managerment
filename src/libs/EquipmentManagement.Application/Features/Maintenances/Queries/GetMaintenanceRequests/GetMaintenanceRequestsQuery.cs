using EquipmentManagement.Application.Features.Maintenances.DTOs;
using MediatR;

namespace EquipmentManagement.Application.Features.Maintenances.Queries.GetMaintenanceRequests;

/// <summary>
/// Query to get paginated list of maintenance requests with filters
/// </summary>
public class GetMaintenanceRequestsQuery : IRequest<GetMaintenanceRequestsQueryResult>
{
    /// <summary>
    /// Page number (starts from 1)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Filter by equipment ID
    /// </summary>
    public Guid? EquipmentId { get; set; }

    /// <summary>
    /// Filter by technician ID
    /// </summary>
    public string? TechnicianId { get; set; }

    /// <summary>
    /// Filter by status (1=Pending, 2=InProgress, 3=Completed, 4=Cancelled)
    /// </summary>
    public int? Status { get; set; }
}
