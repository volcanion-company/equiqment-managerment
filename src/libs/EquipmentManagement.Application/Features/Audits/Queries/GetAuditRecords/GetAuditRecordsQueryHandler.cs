using EquipmentManagement.Application.Features.Audits.DTOs;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Audits.Queries.GetAuditRecords;

public class GetAuditRecordsQueryHandler : IRequestHandler<GetAuditRecordsQuery, GetAuditRecordsQueryResult>
{
    private readonly IAuditRecordRepository _auditRecordRepository;

    public GetAuditRecordsQueryHandler(IAuditRecordRepository auditRecordRepository)
    {
        _auditRecordRepository = auditRecordRepository;
    }

    public async Task<GetAuditRecordsQueryResult> Handle(GetAuditRecordsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _auditRecordRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.EquipmentId,
            cancellationToken);

        // Apply additional filters in memory (can be moved to repository for better performance)
        var filteredItems = items.AsEnumerable();

        if (request.Result.HasValue)
        {
            filteredItems = filteredItems.Where(a => a.Result == request.Result.Value);
        }

        if (request.FromDate.HasValue)
        {
            filteredItems = filteredItems.Where(a => a.CheckDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            filteredItems = filteredItems.Where(a => a.CheckDate <= request.ToDate.Value);
        }

        var auditDtos = filteredItems.Adapt<List<AuditRecordDto>>();

        return new GetAuditRecordsQueryResult
        {
            Items = auditDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
