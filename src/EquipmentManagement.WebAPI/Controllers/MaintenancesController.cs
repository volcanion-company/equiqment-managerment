using EquipmentManagement.Application.Features.Maintenances.Commands.CreateMaintenanceRequest;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManagement.WebAPI.Controllers;

/// <summary>
/// Handles HTTP requests related to maintenance operations.
/// </summary>
/// <remarks>This controller provides endpoints for creating and managing maintenance requests. All routes are
/// prefixed with 'api/maintenances'.</remarks>
/// <param name="mediator">The mediator used to send commands and queries to the application's business logic layer.</param>
/// <param name="logger">The logger used to record information and errors related to maintenance operations.</param>
[ApiController]
[Route("api/[controller]")]
[Tags("Maintenances")]
public class MaintenancesController(IMediator mediator, ILogger<MaintenancesController> logger) : ControllerBase
{
    /// <summary>
    /// Creates a new maintenance request and returns the unique identifier of the created request.
    /// </summary>
    /// <param name="command">The command containing the details of the maintenance request to create. Cannot be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An ActionResult containing the unique identifier of the newly created maintenance request. Returns a 201 Created
    /// response on success, or a 400 Bad Request response if the input is invalid.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateMaintenanceRequest([FromBody] CreateMaintenanceRequestCommand command, CancellationToken cancellationToken)
    {
        // Log the creation attempt
        logger.LogDebug("Received request to create maintenance request for EquipmentId: {EquipmentId}", command.EquipmentId);
        // Send the command to the mediator to handle the creation logic
        var id = await mediator.Send(command, cancellationToken);
        // Log the successful creation
        return CreatedAtAction(nameof(CreateMaintenanceRequest), new { id }, id);
    }
}
