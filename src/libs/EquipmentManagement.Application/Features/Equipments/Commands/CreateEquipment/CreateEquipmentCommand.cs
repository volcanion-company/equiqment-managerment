using EquipmentManagement.Domain.Enums;
using MediatR;

namespace EquipmentManagement.Application.Features.Equipments.Commands.CreateEquipment;

public class CreateEquipmentCommand : IRequest<Guid>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Specification { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string? Supplier { get; set; }
    public decimal Price { get; set; }
    public DateTime? WarrantyEndDate { get; set; }
    public EquipmentStatus Status { get; set; }
    public string? ImageUrl { get; set; }
}
