using EquipmentManagement.Application.Features.Equipments.DTOs;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Equipments.Queries.GetEquipmentById;

public class GetEquipmentByIdQueryHandler : IRequestHandler<GetEquipmentByIdQuery, EquipmentDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEquipmentByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<EquipmentDto?> Handle(GetEquipmentByIdQuery request, CancellationToken cancellationToken)
    {
        var equipment = await _unitOfWork.Equipments.GetByIdAsync(request.Id, cancellationToken);
        return equipment?.Adapt<EquipmentDto>();
    }
}
