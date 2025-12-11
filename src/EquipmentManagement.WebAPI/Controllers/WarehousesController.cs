using EquipmentManagement.Application.Features.Warehouses.Commands.CreateTransaction;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManagement.WebAPI.Controllers;

/// <summary>
/// Handles HTTP requests related to warehouse operations, such as creating warehouse transactions.
/// </summary>
/// <param name="mediator">The mediator used to send commands and queries to the application's business logic layer.</param>
/// <param name="logger">The logger used to record diagnostic and operational information for the controller.</param>
[ApiController]
[Route("api/[controller]")]
[Tags("Warehouses")]
public class WarehousesController(IMediator mediator, ILogger<WarehousesController> logger) : ControllerBase
{
    /// <summary>
    /// Creates a new warehouse transaction and returns the unique identifier of the created transaction.
    /// </summary>
    /// <param name="command">The command containing the details of the warehouse transaction to create. Cannot be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A 201 Created response containing the unique identifier of the newly created transaction if successful;
    /// otherwise, a 400 Bad Request response if the input is invalid.</returns>
    [HttpPost("transactions")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateTransaction([FromBody] CreateWarehouseTransactionCommand command, CancellationToken cancellationToken)
    {
        // Log the incoming request for creating a warehouse transaction
        logger.LogDebug("Received request to create warehouse transaction: {@Command}", command);
        // Send the command to the mediator to handle the creation of the warehouse transaction
        var id = await mediator.Send(command, cancellationToken);
        // Return a 201 Created response with the ID of the newly created transaction
        return CreatedAtAction(nameof(CreateTransaction), new { id }, id);
    }
}
