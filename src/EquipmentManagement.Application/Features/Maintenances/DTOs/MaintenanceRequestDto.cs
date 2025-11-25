using EquipmentManagement.Domain.Enums;

namespace EquipmentManagement.Application.Features.Maintenances.DTOs;

public class MaintenanceRequestDto
{
    public Guid Id { get; set; }
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
    public DateTime CreatedAt { get; set; }
}
