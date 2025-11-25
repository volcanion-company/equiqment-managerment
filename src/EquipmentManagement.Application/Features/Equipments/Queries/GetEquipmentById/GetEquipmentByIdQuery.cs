using EquipmentManagement.Application.Features.Equipments.DTOs;
using MediatR;

namespace EquipmentManagement.Application.Features.Equipments.Queries.GetEquipmentById;

public class GetEquipmentByIdQuery : IRequest<EquipmentDto?>
{
    public Guid Id { get; set; }
}
