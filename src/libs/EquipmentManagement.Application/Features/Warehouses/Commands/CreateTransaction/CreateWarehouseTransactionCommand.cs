using EquipmentManagement.Domain.Enums;
using MediatR;

namespace EquipmentManagement.Application.Features.Warehouses.Commands.CreateTransaction;

public class CreateWarehouseTransactionCommand : IRequest<Guid>
{
    public Guid WarehouseItemId { get; set; }
    public WarehouseTransactionType Type { get; set; }
    public int Quantity { get; set; }
    public string? Reason { get; set; }
    public string PerformedBy { get; set; } = string.Empty;
}
