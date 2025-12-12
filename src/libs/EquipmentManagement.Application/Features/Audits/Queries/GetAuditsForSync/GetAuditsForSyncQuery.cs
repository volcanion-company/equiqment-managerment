using EquipmentManagement.Application.Features.Audits.DTOs;
using MediatR;

namespace EquipmentManagement.Application.Features.Audits.Queries.GetAuditsForSync;

public class GetAuditsForSyncQuery : IRequest<IEnumerable<AuditRecordDto>>
{
    public DateTime? SinceDate { get; set; }
}
