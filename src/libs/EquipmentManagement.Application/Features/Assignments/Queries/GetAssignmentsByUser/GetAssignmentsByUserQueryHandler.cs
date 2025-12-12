using EquipmentManagement.Application.Features.Assignments.DTOs;
using EquipmentManagement.Domain.Enums;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Assignments.Queries.GetAssignmentsByUser;

public class GetAssignmentsByUserQueryHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<GetAssignmentsByUserQuery, IEnumerable<AssignmentDto>>
{
    public async Task<IEnumerable<AssignmentDto>> Handle(GetAssignmentsByUserQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.Assignment> assignments;

        if (request.ActiveOnly)
        {
            assignments = await unitOfWork.Assignments.GetActiveAssignmentsByUserIdAsync(request.UserId, cancellationToken);
        }
        else
        {
            // Get all assignments for this user (active and returned)
            var allAssignments = await unitOfWork.Assignments.GetAllAsync(cancellationToken);
            assignments = allAssignments
                .Where(a => !a.IsDeleted && a.AssignedToUserId == request.UserId)
                .OrderByDescending(a => a.AssignedDate);
        }

        return assignments.Adapt<IEnumerable<AssignmentDto>>();
    }
}
