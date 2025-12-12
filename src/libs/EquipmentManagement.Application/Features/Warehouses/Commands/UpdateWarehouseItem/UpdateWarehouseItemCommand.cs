using MediatR;

namespace EquipmentManagement.Application.Features.Warehouses.Commands.UpdateWarehouseItem;

public class UpdateWarehouseItemCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string EquipmentType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int MinThreshold { get; set; }
    public string? Notes { get; set; }
}
