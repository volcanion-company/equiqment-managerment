using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Application.Common.Interfaces;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Repositories;
using MediatR;

namespace EquipmentManagement.Application.Features.Equipments.Commands.UpdateEquipment;

public class UpdateEquipmentCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService) : IRequestHandler<UpdateEquipmentCommand, Unit>
{
    public async Task<Unit> Handle(UpdateEquipmentCommand request, CancellationToken cancellationToken)
    {
        var equipment = await unitOfWork.Equipments.GetByIdAsync(request.Id, cancellationToken);
        
        if (equipment == null || equipment.IsDeleted)
        {
            throw new NotFoundException(nameof(Equipment), request.Id);
        }

        equipment.Code = request.Code;
        equipment.Name = request.Name;
        equipment.Type = request.Type;
        equipment.Description = request.Description;
        equipment.Specification = request.Specification;
        equipment.PurchaseDate = request.PurchaseDate;
        equipment.Supplier = request.Supplier;
        equipment.Price = request.Price;
        equipment.WarrantyEndDate = request.WarrantyEndDate;
        equipment.Status = request.Status;
        equipment.ImageUrl = request.ImageUrl;
        equipment.UpdatedAt = DateTime.UtcNow;

        unitOfWork.Equipments.Update(equipment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate cache
        await cacheService.RemoveByPrefixAsync("equipments_", cancellationToken);

        return Unit.Value;
    }
}
