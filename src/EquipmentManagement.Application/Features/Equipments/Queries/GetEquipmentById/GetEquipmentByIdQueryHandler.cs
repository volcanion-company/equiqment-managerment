using EquipmentManagement.Application.Features.Equipments.DTOs;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Equipments.Queries.GetEquipmentById;

public class GetEquipmentByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetEquipmentByIdQuery, EquipmentDto?>
{
    public async Task<EquipmentDto?> Handle(GetEquipmentByIdQuery request, CancellationToken cancellationToken)
    {
        var equipment = await unitOfWork.Equipments.GetByIdAsync(request.Id, cancellationToken);
        return equipment?.Adapt<EquipmentDto>();
    }
}
