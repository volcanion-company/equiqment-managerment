using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Enums;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Assignments.Commands.CreateAssignment;

public class CreateAssignmentCommandHandler : IRequestHandler<CreateAssignmentCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateAssignmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var equipment = await _unitOfWork.Equipments.GetByIdAsync(request.EquipmentId, cancellationToken);
        
        if (equipment == null || equipment.IsDeleted)
        {
            throw new NotFoundException(nameof(Equipment), request.EquipmentId);
        }

        // Update equipment status
        equipment.Status = EquipmentStatus.InUse;
        equipment.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Equipments.Update(equipment);

        var assignment = request.Adapt<Assignment>();
        assignment.Id = Guid.NewGuid();
        assignment.Status = AssignmentStatus.Assigned;
        assignment.CreatedAt = DateTime.UtcNow;
        assignment.IsDeleted = false;

        await _unitOfWork.Assignments.AddAsync(assignment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return assignment.Id;
    }
}
