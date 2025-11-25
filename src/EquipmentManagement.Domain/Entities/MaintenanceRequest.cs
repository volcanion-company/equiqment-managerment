using EquipmentManagement.Domain.Common;
using EquipmentManagement.Domain.Enums;

namespace EquipmentManagement.Domain.Entities;

public class MaintenanceRequest : BaseEntity
{
    public Guid EquipmentId { get; set; }
    public string? RequesterId { get; set; }
    public string? TechnicianId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal? Cost { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public MaintenanceStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime RequestDate { get; set; }

    // Navigation properties
    public Equipment Equipment { get; set; } = null!;
}
