using MediatR;

namespace EquipmentManagement.Application.Features.Maintenances.Commands.CreateMaintenanceRequest;

public class CreateMaintenanceRequestCommand : IRequest<Guid>
{
    public Guid EquipmentId { get; set; }
    public string? RequesterId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
