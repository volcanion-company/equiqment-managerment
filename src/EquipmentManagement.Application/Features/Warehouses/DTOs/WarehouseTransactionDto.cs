using EquipmentManagement.Domain.Enums;

namespace EquipmentManagement.Application.Features.Warehouses.DTOs;

public class WarehouseTransactionDto
{
    public Guid Id { get; set; }
    public Guid WarehouseItemId { get; set; }
    public WarehouseTransactionType Type { get; set; }
    public int Quantity { get; set; }
    public string? Reason { get; set; }
    public string? PerformedBy { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
