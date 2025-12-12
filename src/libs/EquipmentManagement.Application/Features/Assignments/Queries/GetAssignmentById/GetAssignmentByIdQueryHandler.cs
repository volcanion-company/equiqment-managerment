using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Application.Features.Assignments.DTOs;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Assignments.Queries.GetAssignmentById;

public class GetAssignmentByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAssignmentByIdQuery, AssignmentDto>
{
    public async Task<AssignmentDto> Handle(GetAssignmentByIdQuery request, CancellationToken cancellationToken)
    {
        var assignment = await unitOfWork.Assignments.GetByIdAsync(request.AssignmentId, cancellationToken);
        
        if (assignment == null || assignment.IsDeleted)
        {
            throw new NotFoundException(nameof(Assignment), request.AssignmentId);
        }

        return assignment.Adapt<AssignmentDto>();
    }
}
