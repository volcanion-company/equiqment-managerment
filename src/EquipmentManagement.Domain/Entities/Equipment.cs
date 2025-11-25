using EquipmentManagement.Domain.Common;
using EquipmentManagement.Domain.Enums;

namespace EquipmentManagement.Domain.Entities;

public class Equipment : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Specification { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string? Supplier { get; set; }
    public decimal Price { get; set; }
    public DateTime? WarrantyEndDate { get; set; }
    public EquipmentStatus Status { get; set; }
    public string? ImageUrl { get; set; }
    public string? QRCodeBase64 { get; set; }

    // Navigation properties
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
    public ICollection<AuditRecord> AuditRecords { get; set; } = new List<AuditRecord>();
    public ICollection<LiquidationRequest> LiquidationRequests { get; set; } = new List<LiquidationRequest>();
}
