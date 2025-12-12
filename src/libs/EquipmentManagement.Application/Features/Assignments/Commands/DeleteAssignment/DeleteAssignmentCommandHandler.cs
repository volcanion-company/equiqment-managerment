using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Enums;
using EquipmentManagement.Domain.Repositories;
using MediatR;

namespace EquipmentManagement.Application.Features.Assignments.Commands.DeleteAssignment;

public class DeleteAssignmentCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteAssignmentCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await unitOfWork.Assignments.GetByIdAsync(request.AssignmentId, cancellationToken);
        
        if (assignment == null || assignment.IsDeleted)
        {
            throw new NotFoundException(nameof(Assignment), request.AssignmentId);
        }

        // Only allow deletion of returned assignments or cancelled ones
        if (assignment.Status == AssignmentStatus.Assigned)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "Status", new[] { "Cannot delete active assignments. Please return the assignment first." } }
            });
        }

        // Soft delete
        assignment.IsDeleted = true;
        assignment.UpdatedAt = DateTime.UtcNow;
        unitOfWork.Assignments.Update(assignment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
