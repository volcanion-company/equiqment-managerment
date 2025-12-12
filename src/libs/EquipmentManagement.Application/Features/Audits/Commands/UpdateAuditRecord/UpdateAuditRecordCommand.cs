using EquipmentManagement.Domain.Enums;
using MediatR;

namespace EquipmentManagement.Application.Features.Audits.Commands.UpdateAuditRecord;

public class UpdateAuditRecordCommand : IRequest<Unit>
{
    public Guid AuditRecordId { get; set; }
    public AuditResult Result { get; set; }
    public string? Note { get; set; }
    public string? Location { get; set; }
}
