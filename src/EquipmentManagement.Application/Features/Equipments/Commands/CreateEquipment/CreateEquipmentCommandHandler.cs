using EquipmentManagement.Application.Common.Interfaces;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Equipments.Commands.CreateEquipment;

public class CreateEquipmentCommandHandler : IRequestHandler<CreateEquipmentCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IQRCodeService _qrCodeService;
    private readonly ICacheService _cacheService;

    public CreateEquipmentCommandHandler(
        IUnitOfWork unitOfWork,
        IQRCodeService qrCodeService,
        ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _qrCodeService = qrCodeService;
        _cacheService = cacheService;
    }

    public async Task<Guid> Handle(CreateEquipmentCommand request, CancellationToken cancellationToken)
    {
        var equipment = request.Adapt<Equipment>();
        equipment.Id = Guid.NewGuid();
        equipment.CreatedAt = DateTime.UtcNow;
        equipment.IsDeleted = false;

        // Generate QR Code
        equipment.QRCodeBase64 = _qrCodeService.GenerateQRCode(equipment.Code);

        await _unitOfWork.Equipments.AddAsync(equipment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate cache
        await _cacheService.RemoveByPrefixAsync("equipments_", cancellationToken);

        return equipment.Id;
    }
}
