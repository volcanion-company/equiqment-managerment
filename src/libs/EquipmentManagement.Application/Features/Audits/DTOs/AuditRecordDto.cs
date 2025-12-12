using EquipmentManagement.Domain.Enums;

namespace EquipmentManagement.Application.Features.Audits.DTOs;

public class AuditRecordDto
{
    public Guid Id { get; set; }
    public Guid EquipmentId { get; set; }
    public DateTime CheckDate { get; set; }
    public string? CheckedByUserId { get; set; }
    public AuditResult Result { get; set; }
    public string? Note { get; set; }
    public string? Location { get; set; }
    public DateTime? LastSyncDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
