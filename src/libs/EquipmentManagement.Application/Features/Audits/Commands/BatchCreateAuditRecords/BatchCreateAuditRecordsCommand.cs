using EquipmentManagement.Domain.Enums;
using MediatR;

namespace EquipmentManagement.Application.Features.Audits.Commands.BatchCreateAuditRecords;

public class BatchCreateAuditRecordsCommand : IRequest<BatchCreateAuditRecordsResult>
{
    public List<AuditRecordInput> AuditRecords { get; set; } = new();
}

public class AuditRecordInput
{
    public Guid EquipmentId { get; set; }
    public DateTime CheckDate { get; set; }
    public string? CheckedByUserId { get; set; }
    public AuditResult Result { get; set; }
    public string? Note { get; set; }
    public string? Location { get; set; }
}

public class BatchCreateAuditRecordsResult
{
    public int TotalRecords { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<Guid> CreatedIds { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}
