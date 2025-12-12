namespace EquipmentManagement.Application.Features.Warehouses.DTOs;

public class WarehouseItemDto
{
    public Guid Id { get; set; }
    public string EquipmentType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int MinThreshold { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
