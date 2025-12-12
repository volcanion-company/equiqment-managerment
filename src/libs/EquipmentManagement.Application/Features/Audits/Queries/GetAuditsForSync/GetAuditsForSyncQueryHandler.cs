using EquipmentManagement.Application.Features.Audits.DTOs;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Audits.Queries.GetAuditsForSync;

public class GetAuditsForSyncQueryHandler : IRequestHandler<GetAuditsForSyncQuery, IEnumerable<AuditRecordDto>>
{
    private readonly IAuditRecordRepository _auditRecordRepository;

    public GetAuditsForSyncQueryHandler(IAuditRecordRepository auditRecordRepository)
    {
        _auditRecordRepository = auditRecordRepository;
    }

    public async Task<IEnumerable<AuditRecordDto>> Handle(GetAuditsForSyncQuery request, CancellationToken cancellationToken)
    {
        // Get all audit records (can be filtered by LastSyncDate for incremental sync)
        var query = await _auditRecordRepository.GetAllAsync(cancellationToken);

        // Filter by sync date if provided (for incremental sync)
        if (request.SinceDate.HasValue)
        {
            query = query.Where(a => a.LastSyncDate > request.SinceDate.Value);
        }

        // Order by CheckDate descending for most recent audits first
        var auditRecords = query.OrderByDescending(a => a.CheckDate).ToList();

        return auditRecords.Adapt<IEnumerable<AuditRecordDto>>();
    }
}
