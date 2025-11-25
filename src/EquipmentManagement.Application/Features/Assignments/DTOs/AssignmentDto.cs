using EquipmentManagement.Domain.Enums;

namespace EquipmentManagement.Application.Features.Assignments.DTOs;

public class AssignmentDto
{
    public Guid Id { get; set; }
    public Guid EquipmentId { get; set; }
    public string? AssignedToUserId { get; set; }
    public string? AssignedToDepartment { get; set; }
    public DateTime AssignedDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public AssignmentStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? AssignedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
