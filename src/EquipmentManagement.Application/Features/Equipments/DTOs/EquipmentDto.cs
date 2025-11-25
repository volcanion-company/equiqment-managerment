using EquipmentManagement.Domain.Enums;

namespace EquipmentManagement.Application.Features.Equipments.DTOs;

public class EquipmentDto
{
    public Guid Id { get; set; }
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
    public string? QRCodeBase64 { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
