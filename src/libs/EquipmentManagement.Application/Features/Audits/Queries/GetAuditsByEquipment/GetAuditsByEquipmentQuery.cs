using EquipmentManagement.Application.Features.Audits.DTOs;
using MediatR;

namespace EquipmentManagement.Application.Features.Audits.Queries.GetAuditsByEquipment;

public class GetAuditsByEquipmentQuery : IRequest<IEnumerable<AuditRecordDto>>
{
    public Guid EquipmentId { get; set; }
}
