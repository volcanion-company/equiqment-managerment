using EquipmentManagement.Application.Features.Audits.DTOs;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Audits.Queries.GetAuditsByEquipment;

public class GetAuditsByEquipmentQueryHandler : IRequestHandler<GetAuditsByEquipmentQuery, IEnumerable<AuditRecordDto>>
{
    private readonly IAuditRecordRepository _auditRecordRepository;

    public GetAuditsByEquipmentQueryHandler(IAuditRecordRepository auditRecordRepository)
    {
        _auditRecordRepository = auditRecordRepository;
    }

    public async Task<IEnumerable<AuditRecordDto>> Handle(GetAuditsByEquipmentQuery request, CancellationToken cancellationToken)
    {
        var auditRecords = await _auditRecordRepository.GetByEquipmentIdAsync(request.EquipmentId, cancellationToken);

        return auditRecords.Adapt<IEnumerable<AuditRecordDto>>();
    }
}
