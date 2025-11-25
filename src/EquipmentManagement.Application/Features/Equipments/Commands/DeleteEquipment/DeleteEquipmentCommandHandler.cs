using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Application.Common.Interfaces;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Repositories;
using MediatR;

namespace EquipmentManagement.Application.Features.Equipments.Commands.DeleteEquipment;

public class DeleteEquipmentCommandHandler : IRequestHandler<DeleteEquipmentCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public DeleteEquipmentCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Unit> Handle(DeleteEquipmentCommand request, CancellationToken cancellationToken)
    {
        var equipment = await _unitOfWork.Equipments.GetByIdAsync(request.Id, cancellationToken);
        
        if (equipment == null || equipment.IsDeleted)
        {
            throw new NotFoundException(nameof(Equipment), request.Id);
        }

        // Soft delete
        equipment.IsDeleted = true;
        equipment.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Equipments.Update(equipment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate cache
        await _cacheService.RemoveByPrefixAsync("equipments_", cancellationToken);

        return Unit.Value;
    }
}
