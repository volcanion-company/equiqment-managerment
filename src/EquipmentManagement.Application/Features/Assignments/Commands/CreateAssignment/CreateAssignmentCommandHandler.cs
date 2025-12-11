using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Enums;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Assignments.Commands.CreateAssignment;

public class CreateAssignmentCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateAssignmentCommand, Guid>
{
    public async Task<Guid> Handle(CreateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var equipment = await unitOfWork.Equipments.GetByIdAsync(request.EquipmentId, cancellationToken);
        
        if (equipment == null || equipment.IsDeleted)
        {
            throw new NotFoundException(nameof(Equipment), request.EquipmentId);
        }

        // Update equipment status
        equipment.Status = EquipmentStatus.InUse;
        equipment.UpdatedAt = DateTime.UtcNow;
        unitOfWork.Equipments.Update(equipment);

        var assignment = request.Adapt<Assignment>();
        assignment.Id = Guid.NewGuid();
        assignment.Status = AssignmentStatus.Assigned;
        assignment.CreatedAt = DateTime.UtcNow;
        assignment.IsDeleted = false;

        await unitOfWork.Assignments.AddAsync(assignment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return assignment.Id;
    }
}
