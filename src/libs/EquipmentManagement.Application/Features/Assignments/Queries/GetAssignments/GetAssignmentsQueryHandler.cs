using EquipmentManagement.Application.Features.Assignments.DTOs;
using EquipmentManagement.Domain.Enums;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Assignments.Queries.GetAssignments;

public class GetAssignmentsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAssignmentsQuery, GetAssignmentsQueryResult>
{
    public async Task<GetAssignmentsQueryResult> Handle(GetAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var (assignments, totalCount) = await unitOfWork.Assignments.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.EquipmentId,
            request.UserId,
            cancellationToken);

        // Filter by status if provided
        var filteredAssignments = assignments;
        if (request.Status.HasValue)
        {
            filteredAssignments = assignments.Where(a => (int)a.Status == request.Status.Value);
            totalCount = filteredAssignments.Count();
        }

        var assignmentDtos = filteredAssignments.Adapt<IEnumerable<AssignmentDto>>();

        return new GetAssignmentsQueryResult
        {
            Items = assignmentDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
