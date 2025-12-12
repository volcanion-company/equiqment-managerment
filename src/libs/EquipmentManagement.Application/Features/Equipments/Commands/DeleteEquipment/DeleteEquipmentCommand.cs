using MediatR;

namespace EquipmentManagement.Application.Features.Equipments.Commands.DeleteEquipment;

public class DeleteEquipmentCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
