using EquipmentManagement.Application.Features.Audits.DTOs;
using EquipmentManagement.Domain.Enums;
using MediatR;

namespace EquipmentManagement.Application.Features.Audits.Queries.GetAuditRecords;

public class GetAuditRecordsQuery : IRequest<GetAuditRecordsQueryResult>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? EquipmentId { get; set; }
    public AuditResult? Result { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class GetAuditRecordsQueryResult
{
    public List<AuditRecordDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
