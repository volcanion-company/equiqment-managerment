using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Application.Common.Interfaces;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Repositories;
using MediatR;

namespace EquipmentManagement.Application.Features.Equipments.Commands.DeleteEquipment;

public class DeleteEquipmentCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService) : IRequestHandler<DeleteEquipmentCommand, Unit>
{
    public async Task<Unit> Handle(DeleteEquipmentCommand request, CancellationToken cancellationToken)
    {
        var equipment = await unitOfWork.Equipments.GetByIdAsync(request.Id, cancellationToken);
        
        if (equipment == null || equipment.IsDeleted)
        {
            throw new NotFoundException(nameof(Equipment), request.Id);
        }

        // Soft delete
        equipment.IsDeleted = true;
        equipment.UpdatedAt = DateTime.UtcNow;

        unitOfWork.Equipments.Update(equipment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate cache
        await cacheService.RemoveByPrefixAsync("equipments_", cancellationToken);

        return Unit.Value;
    }
}
