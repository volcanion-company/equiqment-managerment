using EquipmentManagement.Application.Features.Liquidations.Commands.ApproveLiquidation;
using EquipmentManagement.Application.Features.Liquidations.Commands.CreateLiquidationRequest;
using EquipmentManagement.Application.Features.Liquidations.Commands.RejectLiquidation;
using EquipmentManagement.Application.Features.Liquidations.Commands.UpdateLiquidationRequest;
using EquipmentManagement.Application.Features.Liquidations.DTOs;
using EquipmentManagement.Application.Features.Liquidations.Queries.GetLiquidationById;
using EquipmentManagement.Application.Features.Liquidations.Queries.GetLiquidationRequests;
using EquipmentManagement.Application.Features.Liquidations.Queries.GetPendingLiquidations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManagement.WebAPI.Controllers;

/// <summary>
/// API controller for managing equipment liquidation requests and approval workflow.
/// </summary>
/// <param name="mediator">The mediator used to send commands and queries to the application layer.</param>
/// <param name="logger">The logger used to record diagnostic and operational information.</param>
[ApiController]
[Route("api/[controller]")]
[Tags("Liquidations")]
public class LiquidationsController(IMediator mediator, ILogger<LiquidationsController> logger) : ControllerBase
{
    /// <summary>
    /// Retrieves a paginated list of liquidation requests with optional filtering.
    /// </summary>
    /// <param name="query">Query parameters including pagination and approval status filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of liquidation requests</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetLiquidationRequestsQueryResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetLiquidationRequestsQueryResult>> GetLiquidationRequests(
        [FromQuery] GetLiquidationRequestsQuery query,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Getting liquidation requests - Page: {PageNumber}, Size: {PageSize}", 
            query.PageNumber, query.PageSize);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all pending liquidation requests awaiting approval.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of pending liquidation requests</returns>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(IEnumerable<LiquidationRequestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LiquidationRequestDto>>> GetPendingLiquidations(
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Getting pending liquidation requests");
        var query = new GetPendingLiquidationsQuery();
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves detailed information about a specific liquidation request.
    /// </summary>
    /// <param name="id">Liquidation request ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Liquidation request details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LiquidationRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LiquidationRequestDto>> GetLiquidationById(
        Guid id,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Getting liquidation request by ID: {LiquidationId}", id);
        var query = new GetLiquidationByIdQuery { LiquidationRequestId = id };
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves liquidation history for a specific equipment.
    /// </summary>
    /// <param name="equipmentId">Equipment ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of liquidation requests for the equipment</returns>
    [HttpGet("equipment/{equipmentId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<LiquidationRequestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LiquidationRequestDto>>> GetLiquidationsByEquipment(
        Guid equipmentId,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Getting liquidation requests for equipment: {EquipmentId}", equipmentId);
        var query = new GetLiquidationRequestsQuery 
        { 
            PageNumber = 1, 
            PageSize = 100 
        };
        var result = await mediator.Send(query, cancellationToken);
        var filtered = result.Items.Where(x => x.EquipmentId == equipmentId);
        return Ok(filtered);
    }

    /// <summary>
    /// Creates a new liquidation request for the specified equipment.
    /// </summary>
    /// <param name="command">The command containing the details required to create the liquidation request. Cannot be null.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A 201 Created response containing the unique identifier of the newly created liquidation request if successful;
    /// otherwise, a 400 Bad Request response if the request is invalid.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateLiquidationRequest(
        [FromBody] CreateLiquidationRequestCommand command, 
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Creating a new liquidation request for EquipmentId: {EquipmentId}", command.EquipmentId);
        var id = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetLiquidationById), new { id }, id);
    }

    /// <summary>
    /// Updates a pending liquidation request.
    /// </summary>
    /// <param name="id">Liquidation request ID</param>
    /// <param name="command">Update command with new values</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>NoContent if successful</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateLiquidationRequest(
        Guid id,
        [FromBody] UpdateLiquidationRequestCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.LiquidationRequestId)
            return BadRequest("ID mismatch");

        logger.LogDebug("Updating liquidation request: {LiquidationId}", id);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Approves a liquidation request and updates equipment status to Liquidated.
    /// </summary>
    /// <param name="id">Liquidation request ID</param>
    /// <param name="command">Approval command with approver details and liquidation value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>NoContent if successful</returns>
    [HttpPut("{id:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveLiquidation(
        Guid id,
        [FromBody] ApproveLiquidationCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.LiquidationRequestId)
            return BadRequest("ID mismatch");

        logger.LogDebug("Approving liquidation request: {LiquidationId}", id);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Rejects a liquidation request with a reason.
    /// </summary>
    /// <param name="id">Liquidation request ID</param>
    /// <param name="command">Rejection command with reason</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>NoContent if successful</returns>
    [HttpPut("{id:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectLiquidation(
        Guid id,
        [FromBody] RejectLiquidationCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.LiquidationRequestId)
            return BadRequest("ID mismatch");

        logger.LogDebug("Rejecting liquidation request: {LiquidationId}", id);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
