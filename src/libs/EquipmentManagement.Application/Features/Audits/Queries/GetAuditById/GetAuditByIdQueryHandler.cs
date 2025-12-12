using EquipmentManagement.Application.Features.Audits.DTOs;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Audits.Queries.GetAuditById;

public class GetAuditByIdQueryHandler : IRequestHandler<GetAuditByIdQuery, AuditRecordDto>
{
    private readonly IAuditRecordRepository _auditRecordRepository;

    public GetAuditByIdQueryHandler(IAuditRecordRepository auditRecordRepository)
    {
        _auditRecordRepository = auditRecordRepository;
    }

    public async Task<AuditRecordDto> Handle(GetAuditByIdQuery request, CancellationToken cancellationToken)
    {
        var auditRecord = await _auditRecordRepository.GetByIdAsync(request.AuditRecordId, cancellationToken)
            ?? throw new KeyNotFoundException($"Audit record with ID {request.AuditRecordId} not found");

        return auditRecord.Adapt<AuditRecordDto>();
    }
}
