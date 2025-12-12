using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Enums;
using EquipmentManagement.Domain.Repositories;
using MediatR;

namespace EquipmentManagement.Application.Features.Assignments.Commands.UpdateAssignment;

public class UpdateAssignmentCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateAssignmentCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await unitOfWork.Assignments.GetByIdAsync(request.AssignmentId, cancellationToken);
        
        if (assignment == null || assignment.IsDeleted)
        {
            throw new NotFoundException(nameof(Assignment), request.AssignmentId);
        }

        // Cannot update returned or lost assignments
        if (assignment.Status != AssignmentStatus.Assigned)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "Status", new[] { "Can only update assignments with 'Assigned' status" } }
            });
        }

        // Update fields
        if (request.AssignedDate.HasValue)
        {
            assignment.AssignedDate = request.AssignedDate.Value;
        }

        if (!string.IsNullOrEmpty(request.Notes))
        {
            assignment.Notes = request.Notes;
        }

        if (!string.IsNullOrEmpty(request.AssignedToUserId))
        {
            assignment.AssignedToUserId = request.AssignedToUserId;
        }

        if (!string.IsNullOrEmpty(request.AssignedToDepartment))
        {
            assignment.AssignedToDepartment = request.AssignedToDepartment;
        }

        assignment.UpdatedAt = DateTime.UtcNow;
        unitOfWork.Assignments.Update(assignment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
