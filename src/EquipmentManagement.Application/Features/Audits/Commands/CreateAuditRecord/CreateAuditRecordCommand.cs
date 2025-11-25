using EquipmentManagement.Domain.Enums;
using MediatR;

namespace EquipmentManagement.Application.Features.Audits.Commands.CreateAuditRecord;

public class CreateAuditRecordCommand : IRequest<Guid>
{
    public Guid EquipmentId { get; set; }
    public DateTime CheckDate { get; set; }
    public string? CheckedByUserId { get; set; }
    public AuditResult Result { get; set; }
    public string? Note { get; set; }
    public string? Location { get; set; }
}
