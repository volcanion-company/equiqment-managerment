using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Liquidations.Commands.CreateLiquidationRequest;

public class CreateLiquidationRequestCommandHandler : IRequestHandler<CreateLiquidationRequestCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateLiquidationRequestCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateLiquidationRequestCommand request, CancellationToken cancellationToken)
    {
        var equipment = await _unitOfWork.Equipments.GetByIdAsync(request.EquipmentId, cancellationToken);
        
        if (equipment == null || equipment.IsDeleted)
        {
            throw new NotFoundException(nameof(Equipment), request.EquipmentId);
        }

        var liquidationRequest = request.Adapt<LiquidationRequest>();
        liquidationRequest.Id = Guid.NewGuid();
        liquidationRequest.RequestDate = DateTime.UtcNow;
        liquidationRequest.IsApproved = false;
        liquidationRequest.CreatedAt = DateTime.UtcNow;
        liquidationRequest.IsDeleted = false;

        await _unitOfWork.LiquidationRequests.AddAsync(liquidationRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return liquidationRequest.Id;
    }
}
