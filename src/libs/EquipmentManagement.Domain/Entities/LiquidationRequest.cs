using EquipmentManagement.Domain.Common;

namespace EquipmentManagement.Domain.Entities;

public class LiquidationRequest : BaseEntity
{
    public Guid EquipmentId { get; set; }
    public string? ApprovedBy { get; set; }
    public decimal? LiquidationValue { get; set; }
    public string? Note { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public bool IsApproved { get; set; }

    // Navigation properties
    public virtual Equipment Equipment { get; set; } = null!;
}
