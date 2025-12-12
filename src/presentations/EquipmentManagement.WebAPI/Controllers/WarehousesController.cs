using EquipmentManagement.Application.Common.Models;
using EquipmentManagement.Application.Features.Warehouses.Commands.CreateTransaction;
using EquipmentManagement.Application.Features.Warehouses.Commands.CreateWarehouseItem;
using EquipmentManagement.Application.Features.Warehouses.Commands.UpdateWarehouseItem;
using EquipmentManagement.Application.Features.Warehouses.Commands.DeleteWarehouseItem;
using EquipmentManagement.Application.Features.Warehouses.DTOs;
using EquipmentManagement.Application.Features.Warehouses.Queries.GetWarehouseItems;
using EquipmentManagement.Application.Features.Warehouses.Queries.GetWarehouseItemById;
using EquipmentManagement.Application.Features.Warehouses.Queries.GetLowStockItems;
using EquipmentManagement.Application.Features.Warehouses.Queries.GetWarehouseTransactions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManagement.WebAPI.Controllers;

/// <summary>
/// Handles HTTP requests related to warehouse operations, including warehouse items and transactions management.
/// </summary>
/// <param name="mediator">The mediator used to send commands and queries to the application's business logic layer.</param>
/// <param name="logger">The logger used to record diagnostic and operational information for the controller.</param>
[ApiController]
[Route("api/[controller]")]
[Tags("Warehouses")]
public class WarehousesController(IMediator mediator, ILogger<WarehousesController> logger) : ControllerBase
{
    #region Warehouse Items

    /// <summary>
    /// Retrieves a paginated list of warehouse items, optionally filtered by equipment type or low stock status.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve. Must be greater than or equal to 1. The default value is 1.</param>
    /// <param name="pageSize">The number of items to include on each page. Must be greater than 0. The default value is 10.</param>
    /// <param name="equipmentType">An optional equipment type to filter the results. If null, no filtering by type is applied.</param>
    /// <param name="lowStockOnly">Filter to show only items with quantity below minimum threshold. If null, all items are shown.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A paginated result containing warehouse items that match the specified filters.</returns>
    [HttpGet("items")]
    [ProducesResponseType(typeof(PagedResult<WarehouseItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<WarehouseItemDto>>> GetWarehouseItems(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? equipmentType = null,
        [FromQuery] bool? lowStockOnly = null,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Fetching warehouse items: PageNumber={PageNumber}, PageSize={PageSize}, EquipmentType={EquipmentType}, LowStockOnly={LowStockOnly}",
            pageNumber, pageSize, equipmentType, lowStockOnly);

        var query = new GetWarehouseItemsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            EquipmentType = equipmentType,
            LowStockOnly = lowStockOnly
        };

        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all warehouse items that have stock levels at or below their minimum threshold.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A list of warehouse items with low stock levels.</returns>
    [HttpGet("items/low-stock")]
    [ProducesResponseType(typeof(List<WarehouseItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<WarehouseItemDto>>> GetLowStockItems(CancellationToken cancellationToken)
    {
        logger.LogDebug("Fetching low stock warehouse items");

        var query = new GetLowStockItemsQuery();
        var result = await mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves the warehouse item with the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the warehouse item to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>The warehouse item data if found; otherwise, a 404 Not Found response.</returns>
    [HttpGet("items/{id}")]
    [ProducesResponseType(typeof(WarehouseItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WarehouseItemDto>> GetWarehouseItem(Guid id, CancellationToken cancellationToken)
    {
        logger.LogDebug("Fetching warehouse item with ID: {Id}", id);

        var query = new GetWarehouseItemByIdQuery { Id = id };
        var result = await mediator.Send(query, cancellationToken);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Creates a new warehouse item for tracking inventory of a specific equipment type.
    /// </summary>
    /// <param name="command">The command containing the details of the warehouse item to create. Cannot be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A 201 Created response containing the unique identifier of the newly created warehouse item if successful;
    /// otherwise, a 400 Bad Request response if the input is invalid.</returns>
    [HttpPost("items")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateWarehouseItem([FromBody] CreateWarehouseItemCommand command, CancellationToken cancellationToken)
    {
        logger.LogDebug("Creating new warehouse item: {@Command}", command);

        var id = await mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetWarehouseItem), new { id }, id);
    }

    /// <summary>
    /// Updates the details of an existing warehouse item with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the warehouse item to update.</param>
    /// <param name="command">An object containing the updated values for the warehouse item. The Id property of this object must match the id parameter.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A NoContentResult if the update is successful; a BadRequestResult if the identifier does not match or the request is invalid;
    /// or a NotFoundResult if the warehouse item does not exist.</returns>
    [HttpPut("items/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateWarehouseItem(Guid id, [FromBody] UpdateWarehouseItemCommand command, CancellationToken cancellationToken)
    {
        logger.LogDebug("Updating warehouse item with ID: {Id}", id);

        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Deletes the warehouse item with the specified identifier (soft delete).
    /// </summary>
    /// <param name="id">The unique identifier of the warehouse item to delete.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A response indicating the result of the delete operation. Returns 204 No Content if the warehouse item was deleted
    /// successfully; otherwise, returns 404 Not Found if the item does not exist.</returns>
    [HttpDelete("items/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteWarehouseItem(Guid id, CancellationToken cancellationToken)
    {
        logger.LogDebug("Deleting warehouse item with ID: {Id}", id);

        var command = new DeleteWarehouseItemCommand { Id = id };
        await mediator.Send(command, cancellationToken);

        return NoContent();
    }

    #endregion

    #region Warehouse Transactions

    /// <summary>
    /// Retrieves a paginated list of warehouse transactions, optionally filtered by warehouse item.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve. Must be greater than or equal to 1. The default value is 1.</param>
    /// <param name="pageSize">The number of items to include on each page. Must be greater than 0. The default value is 10.</param>
    /// <param name="warehouseItemId">An optional warehouse item ID to filter transactions. If null, all transactions are shown.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A paginated result containing warehouse transactions that match the specified filters.</returns>
    [HttpGet("transactions")]
    [ProducesResponseType(typeof(PagedResult<WarehouseTransactionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<WarehouseTransactionDto>>> GetWarehouseTransactions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? warehouseItemId = null,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Fetching warehouse transactions: PageNumber={PageNumber}, PageSize={PageSize}, WarehouseItemId={WarehouseItemId}",
            pageNumber, pageSize, warehouseItemId);

        var query = new GetWarehouseTransactionsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            WarehouseItemId = warehouseItemId
        };

        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

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
        logger.LogDebug("Received request to create warehouse transaction: {@Command}", command);

        var id = await mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(CreateTransaction), new { id }, id);
    }

    #endregion
}
