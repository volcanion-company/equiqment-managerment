using EquipmentManagement.Application.Common.Models;
using EquipmentManagement.Application.Features.Equipments.Commands.CreateEquipment;
using EquipmentManagement.Application.Features.Equipments.Commands.DeleteEquipment;
using EquipmentManagement.Application.Features.Equipments.Commands.UpdateEquipment;
using EquipmentManagement.Application.Features.Equipments.DTOs;
using EquipmentManagement.Application.Features.Equipments.Queries.GetEquipmentById;
using EquipmentManagement.Application.Features.Equipments.Queries.GetEquipments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManagement.WebAPI.Controllers;

/// <summary>
/// API controller for managing equipment resources, providing endpoints to retrieve, create, update, and delete
/// equipment.
/// </summary>
/// <remarks>This controller exposes RESTful endpoints for equipment management, including support for pagination,
/// filtering, and soft deletion. All endpoints require appropriate authorization as configured in the application.
/// Responses follow standard HTTP status codes for success and error conditions.</remarks>
/// <param name="mediator">The mediator used to send commands and queries for equipment operations.</param>
/// <param name="logger">The logger instance used for logging controller operations and events.</param>
[ApiController]
[Route("api/[controller]")]
[Tags("Equipments")]
public class EquipmentsController(IMediator mediator, ILogger<EquipmentsController> logger) : ControllerBase
{
    /// <summary>
    /// Retrieves a paginated list of equipment items, optionally filtered by type, status, or keyword.
    /// </summary>
    /// <remarks>This endpoint supports pagination and filtering to efficiently retrieve large sets of
    /// equipment. Filtering parameters are optional; omitting them returns all equipment. The response includes
    /// pagination metadata such as total item count and page information.</remarks>
    /// <param name="pageNumber">The page number to retrieve. Must be greater than or equal to 1. The default value is 1.</param>
    /// <param name="pageSize">The number of items to include on each page. Must be greater than 0. The default value is 10.</param>
    /// <param name="type">An optional equipment type to filter the results. If null, no filtering by type is applied.</param>
    /// <param name="status">An optional equipment status to filter the results. If null, no filtering by status is applied.</param>
    /// <param name="keyword">An optional keyword to search for in equipment records. If null, no keyword filtering is applied.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An ActionResult containing a paged result of equipment data transfer objects that match the specified filters.
    /// Returns an empty result if no equipment matches the criteria.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EquipmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<EquipmentDto>>> GetEquipments(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? type = null,
        [FromQuery] string? status = null,
        [FromQuery] string? keyword = null,
        CancellationToken cancellationToken = default)
    {
        // Log the request parameters
        logger.LogDebug("Fetching equipments: PageNumber={PageNumber}, PageSize={PageSize}, Type={Type}, Status={Status}, Keyword={Keyword}",
            pageNumber, pageSize, type, status, keyword);
        // Create and send the query
        var query = new GetEquipmentsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Type = type,
            Status = status,
            Keyword = keyword
        };
        // Send the query to the mediator
        var result = await mediator.Send(query, cancellationToken);
        // Return the paginated result
        return Ok(result);
    }

    /// <summary>
    /// Retrieves the equipment item with the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the equipment to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="ActionResult{T}"/> containing the equipment data if found; otherwise, a 404 Not Found response.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EquipmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EquipmentDto>> GetEquipment(Guid id, CancellationToken cancellationToken)
    {
        // Log the request for fetching equipment by ID
        logger.LogDebug("Fetching equipment with ID: {Id}", id);
        // Create and send the query to get equipment by ID
        var query = new GetEquipmentByIdQuery { Id = id };
        // Send the query to the mediator
        var result = await mediator.Send(query, cancellationToken);
        if (result == null)
        {
            // Equipment not found, return 404
            return NotFound();
        }
        // Return the found equipment
        return Ok(result);
    }

    /// <summary>
    /// Creates a new equipment item using the specified command.
    /// </summary>
    /// <param name="command">The command containing the details required to create the equipment item. Cannot be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An ActionResult containing the unique identifier of the newly created equipment item if the operation is
    /// successful; otherwise, a Bad Request response if the input is invalid.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateEquipment([FromBody] CreateEquipmentCommand command, CancellationToken cancellationToken)
    {
        // Log the creation request
        logger.LogDebug("Creating new equipment with Code: {Code}, Name: {Name}", command.Code, command.Name);
        // Send the create equipment command to the mediator
        var id = await mediator.Send(command, cancellationToken);
        // Return the created response with the new equipment ID
        return CreatedAtAction(nameof(GetEquipment), new { id }, id);
    }

    /// <summary>
    /// Updates the details of an existing equipment item with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the equipment item to update.</param>
    /// <param name="command">An object containing the updated values for the equipment item. The <c>Id</c> property of this object must match
    /// the <paramref name="id"/> parameter.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="NoContentResult"/> if the update is successful; a <see cref="BadRequestResult"/> if the identifier
    /// does not match or the request is invalid; or a <see cref="NotFoundResult"/> if the equipment item does not
    /// exist.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEquipment(Guid id, [FromBody] UpdateEquipmentCommand command, CancellationToken cancellationToken)
    {
        // Log the update request
        logger.LogDebug("Updating equipment with ID: {Id}", id);
        // Validate that the ID in the route matches the ID in the command
        if (id != command.Id)
        {
            // Return a bad request response if there is a mismatch
            return BadRequest("ID mismatch");
        }
        // Send the update equipment command to the mediator
        await mediator.Send(command, cancellationToken);
        // Return a no content response to indicate success
        return NoContent();
    }

    /// <summary>
    /// Deletes the equipment item with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the equipment to delete.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A response indicating the result of the delete operation. Returns 204 No Content if the equipment was deleted
    /// successfully, or 404 Not Found if no equipment with the specified identifier exists.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEquipment(Guid id, CancellationToken cancellationToken)
    {
        // Log the delete request
        logger.LogDebug("Deleting equipment with ID: {Id}", id);
        // Create and send the delete equipment command
        var command = new DeleteEquipmentCommand { Id = id };
        // Send the command to the mediator
        await mediator.Send(command, cancellationToken);
        // Return a no content response to indicate successful deletion
        return NoContent();
    }
}
