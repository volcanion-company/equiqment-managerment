using EquipmentManagement.Domain.Common;

namespace EquipmentManagement.Domain.Entities;

public class WarehouseItem : BaseEntity
{
    public string EquipmentType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int MinThreshold { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public virtual ICollection<WarehouseTransaction> Transactions { get; set; } = [];
}
