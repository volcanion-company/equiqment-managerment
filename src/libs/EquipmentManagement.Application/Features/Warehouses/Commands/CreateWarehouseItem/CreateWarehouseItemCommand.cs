using MediatR;

namespace EquipmentManagement.Application.Features.Warehouses.Commands.CreateWarehouseItem;

public class CreateWarehouseItemCommand : IRequest<Guid>
{
    public string EquipmentType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int MinThreshold { get; set; }
    public string? Notes { get; set; }
}
