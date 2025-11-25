using EquipmentManagement.Domain.Common;
using EquipmentManagement.Domain.Enums;

namespace EquipmentManagement.Domain.Entities;

public class Assignment : BaseEntity
{
    public Guid EquipmentId { get; set; }
    public string? AssignedToUserId { get; set; }
    public string? AssignedToDepartment { get; set; }
    public DateTime AssignedDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public AssignmentStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? AssignedBy { get; set; }

    // Navigation properties
    public Equipment Equipment { get; set; } = null!;
}
