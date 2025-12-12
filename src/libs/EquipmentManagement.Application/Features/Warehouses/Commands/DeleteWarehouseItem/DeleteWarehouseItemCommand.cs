using MediatR;

namespace EquipmentManagement.Application.Features.Warehouses.Commands.DeleteWarehouseItem;

public class DeleteWarehouseItemCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
