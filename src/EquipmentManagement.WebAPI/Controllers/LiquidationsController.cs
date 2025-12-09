using EquipmentManagement.Application.Features.Liquidations.Commands.CreateLiquidationRequest;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManagement.WebAPI.Controllers;

/// <summary>
/// API controller for managing liquidation requests.
/// </summary>
/// <param name="mediator">The mediator used to send commands and queries to the application layer.</param>
/// <param name="logger">The logger used to record diagnostic and operational information.</param>
[ApiController]
[Route("api/[controller]")]
[Tags("Liquidations")]
public class LiquidationsController(IMediator mediator, ILogger<LiquidationsController> logger) : ControllerBase
{
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
    public async Task<ActionResult<Guid>> CreateLiquidationRequest([FromBody] CreateLiquidationRequestCommand command, CancellationToken cancellationToken)
    {
        // Log the creation attempt
        logger.LogDebug("Creating a new liquidation request for EquipmentId: {EquipmentId}", command.EquipmentId);
        // Send the command to the mediator to handle the creation logic
        var id = await mediator.Send(command, cancellationToken);
        // Return a 201 Created response with the new liquidation request ID
        return CreatedAtAction(nameof(CreateLiquidationRequest), new { id }, id);
    }
}
