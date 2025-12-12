using EquipmentManagement.Application.Common.Interfaces;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Equipments.Commands.CreateEquipment;

public class CreateEquipmentCommandHandler(
    IUnitOfWork unitOfWork,
    IQRCodeService qrCodeService,
    ICacheService cacheService) : IRequestHandler<CreateEquipmentCommand, Guid>
{
    public async Task<Guid> Handle(CreateEquipmentCommand request, CancellationToken cancellationToken)
    {
        var equipment = request.Adapt<Equipment>();
        equipment.Id = Guid.NewGuid();
        equipment.CreatedAt = DateTime.UtcNow;
        equipment.IsDeleted = false;

        // Generate QR Code
        equipment.QRCodeBase64 = qrCodeService.GenerateQRCode(equipment.Code);

        await unitOfWork.Equipments.AddAsync(equipment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate cache
        await cacheService.RemoveByPrefixAsync("equipments_", cancellationToken);

        return equipment.Id;
    }
}
