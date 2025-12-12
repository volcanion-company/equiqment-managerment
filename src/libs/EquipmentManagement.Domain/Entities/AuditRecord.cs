using EquipmentManagement.Domain.Common;
using EquipmentManagement.Domain.Enums;

namespace EquipmentManagement.Domain.Entities;

public class AuditRecord : BaseEntity
{
    public Guid EquipmentId { get; set; }
    public DateTime CheckDate { get; set; }
    public string? CheckedByUserId { get; set; }
    public AuditResult Result { get; set; }
    public string? Note { get; set; }
    public string? Location { get; set; }
    public DateTime? LastSyncDate { get; set; }

    // Navigation properties
    public virtual Equipment Equipment { get; set; } = null!;
}
