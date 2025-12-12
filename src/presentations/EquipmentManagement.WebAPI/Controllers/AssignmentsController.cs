using EquipmentManagement.Application.Features.Assignments.Commands.CreateAssignment;
using EquipmentManagement.Application.Features.Assignments.Commands.DeleteAssignment;
using EquipmentManagement.Application.Features.Assignments.Commands.ReturnAssignment;
using EquipmentManagement.Application.Features.Assignments.Commands.UpdateAssignment;
using EquipmentManagement.Application.Features.Assignments.DTOs;
using EquipmentManagement.Application.Features.Assignments.Queries.GetAssignmentById;
using EquipmentManagement.Application.Features.Assignments.Queries.GetAssignments;
using EquipmentManagement.Application.Features.Assignments.Queries.GetAssignmentsByUser;
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
    /// Get paginated list of assignments with optional filters
    /// </summary>
    /// <param name="query">Query parameters for pagination and filtering</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of assignments</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetAssignmentsQueryResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GetAssignmentsQueryResult>> GetAssignments([FromQuery] GetAssignmentsQuery query, CancellationToken cancellationToken)
    {
        logger.LogDebug("Getting assignments - Page: {PageNumber}, Size: {PageSize}", query.PageNumber, query.PageSize);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get assignment details by ID
    /// </summary>
    /// <param name="id">Assignment ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Assignment details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AssignmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AssignmentDto>> GetAssignmentById(Guid id, CancellationToken cancellationToken)
    {
        logger.LogDebug("Getting assignment by ID: {AssignmentId}", id);
        var query = new GetAssignmentByIdQuery { AssignmentId = id };
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get assignments for a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="activeOnly">Include only active assignments (default: true)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of user's assignments</returns>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<AssignmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<AssignmentDto>>> GetAssignmentsByUser(
        string userId, 
        [FromQuery] bool activeOnly = true, 
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting assignments for user: {UserId}, ActiveOnly: {ActiveOnly}", userId, activeOnly);
        var query = new GetAssignmentsByUserQuery { UserId = userId, ActiveOnly = activeOnly };
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get assignment history for a specific equipment
    /// </summary>
    /// <param name="equipmentId">Equipment ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of assignments for the equipment</returns>
    [HttpGet("equipment/{equipmentId}")]
    [ProducesResponseType(typeof(GetAssignmentsQueryResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GetAssignmentsQueryResult>> GetAssignmentsByEquipment(
        Guid equipmentId, 
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Getting assignments for equipment: {EquipmentId}", equipmentId);
        var query = new GetAssignmentsQuery { EquipmentId = equipmentId, PageSize = 100 };
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

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
        logger.LogDebug("Creating a new assignment for EquipmentId: {EquipmentId}", command.EquipmentId);
        var id = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetAssignmentById), new { id }, id);
    }

    /// <summary>
    /// Update assignment information
    /// </summary>
    /// <param name="id">Assignment ID</param>
    /// <param name="command">Update command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAssignment(Guid id, [FromBody] UpdateAssignmentCommand command, CancellationToken cancellationToken)
    {
        if (id != command.AssignmentId)
        {
            return BadRequest("Assignment ID mismatch");
        }

        logger.LogDebug("Updating assignment: {AssignmentId}", id);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Return an assigned equipment back to warehouse
    /// </summary>
    /// <param name="id">Assignment ID</param>
    /// <param name="command">Return command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}/return")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReturnAssignment(Guid id, [FromBody] ReturnAssignmentCommand command, CancellationToken cancellationToken)
    {
        if (id != command.AssignmentId)
        {
            return BadRequest("Assignment ID mismatch");
        }

        logger.LogDebug("Returning assignment: {AssignmentId}", id);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Soft delete an assignment (only allowed for returned assignments)
    /// </summary>
    /// <param name="id">Assignment ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAssignment(Guid id, CancellationToken cancellationToken)
    {
        logger.LogDebug("Deleting assignment: {AssignmentId}", id);
        var command = new DeleteAssignmentCommand { AssignmentId = id };
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
