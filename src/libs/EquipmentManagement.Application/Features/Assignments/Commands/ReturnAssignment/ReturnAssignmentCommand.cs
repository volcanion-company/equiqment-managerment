using MediatR;

namespace EquipmentManagement.Application.Features.Assignments.Commands.ReturnAssignment;

/// <summary>
/// Command to return an assigned equipment back to warehouse
/// </summary>
public class ReturnAssignmentCommand : IRequest<Unit>
{
    /// <summary>
    /// Assignment ID to return
    /// </summary>
    public Guid AssignmentId { get; set; }

    /// <summary>
    /// Return notes or condition description
    /// </summary>
    public string? ReturnNotes { get; set; }

    /// <summary>
    /// Person who processed the return
    /// </summary>
    public string? ReturnedBy { get; set; }

    /// <summary>
    /// Indicate if equipment needs maintenance after return
    /// </summary>
    public bool NeedsMaintenance { get; set; }
}
