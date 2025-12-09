using EquipmentManagement.Domain.Common;
using EquipmentManagement.Domain.Enums;

namespace EquipmentManagement.Domain.Entities;

public class WarehouseTransaction : BaseEntity
{
    public Guid WarehouseItemId { get; set; }
    public WarehouseTransactionType Type { get; set; }
    public int Quantity { get; set; }
    public string? Reason { get; set; }
    public string? PerformedBy { get; set; }
    public DateTime TransactionDate { get; set; }

    // Navigation properties
    public virtual WarehouseItem WarehouseItem { get; set; } = null!;
}
