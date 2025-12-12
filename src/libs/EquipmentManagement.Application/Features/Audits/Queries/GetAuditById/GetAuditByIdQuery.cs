using EquipmentManagement.Application.Features.Audits.DTOs;
using MediatR;

namespace EquipmentManagement.Application.Features.Audits.Queries.GetAuditById;

public class GetAuditByIdQuery : IRequest<AuditRecordDto>
{
    public Guid AuditRecordId { get; set; }
}
