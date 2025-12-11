using EquipmentManagement.Application.Features.Assignments.Commands.CreateAssignment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManagement.WebAPI.Controllers;

/// <summary>
/// Handles HTTP requests related to equipment assignments.
/// </summary>
/// <remarks>This controller provides endpoints for creating and managing equipment assignments. All routes are
/// prefixed with 'api/Assignments'.</remarks>
/// <param name="mediator">The mediator used to send commands and queries to the application's business logic layer.</param>
/// <param name="logger">The logger used to record information and errors related to assignment operations.</param>
[ApiController]
[Route("api/[controller]")]
[Tags("Assignments")]
public class AssignmentsController(IMediator mediator, ILogger<AssignmentsController> logger) : ControllerBase
{
    /// <summary>
    /// Creates a new assignment and returns the unique identifier of the created assignment.
    /// </summary>
    /// <param name="command">The command containing the details required to create the assignment. Cannot be null.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An ActionResult containing the unique identifier of the newly created assignment with a 201 Created response on
    /// success, or a 400 Bad Request response if the input is invalid.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateAssignment([FromBody] CreateAssignmentCommand command, CancellationToken cancellationToken)
    {
        // Log the creation attempt
        logger.LogDebug("Creating a new assignment for EquipmentId: {EquipmentId}", command.EquipmentId);
        // Send the command to create a new assignment
        var id = await mediator.Send(command, cancellationToken);
        // Return a 201 Created response with the assignment ID
        return CreatedAtAction(nameof(CreateAssignment), new { id }, id);
    }
}
