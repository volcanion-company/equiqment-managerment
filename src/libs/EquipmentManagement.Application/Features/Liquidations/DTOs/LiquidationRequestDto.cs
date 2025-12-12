namespace EquipmentManagement.Application.Features.Liquidations.DTOs;

public class LiquidationRequestDto
{
    public Guid Id { get; set; }
    public Guid EquipmentId { get; set; }
    public string? ApprovedBy { get; set; }
    public decimal? LiquidationValue { get; set; }
    public string? Note { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }
}
