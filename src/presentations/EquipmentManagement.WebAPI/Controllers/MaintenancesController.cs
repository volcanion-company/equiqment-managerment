using EquipmentManagement.Application.Features.Maintenances.Commands.AssignTechnician;
using EquipmentManagement.Application.Features.Maintenances.Commands.CancelMaintenance;
using EquipmentManagement.Application.Features.Maintenances.Commands.CompleteMaintenance;
using EquipmentManagement.Application.Features.Maintenances.Commands.CreateMaintenanceRequest;
using EquipmentManagement.Application.Features.Maintenances.Commands.StartMaintenance;
using EquipmentManagement.Application.Features.Maintenances.Commands.UpdateMaintenance;
using EquipmentManagement.Application.Features.Maintenances.DTOs;
using EquipmentManagement.Application.Features.Maintenances.Queries.GetMaintenanceById;
using EquipmentManagement.Application.Features.Maintenances.Queries.GetMaintenanceRequests;
using EquipmentManagement.Application.Features.Maintenances.Queries.GetMaintenancesByTechnician;
using EquipmentManagement.Application.Features.Maintenances.Queries.GetPendingMaintenances;
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
    /// Get paginated list of maintenance requests with optional filters
    /// </summary>
    /// <param name="query">Query parameters for pagination and filtering</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of maintenance requests</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetMaintenanceRequestsQueryResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GetMaintenanceRequestsQueryResult>> GetMaintenanceRequests(
        [FromQuery] GetMaintenanceRequestsQuery query, 
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Getting maintenance requests - Page: {PageNumber}, Size: {PageSize}", query.PageNumber, query.PageSize);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get pending maintenance requests (oldest first)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of pending maintenance requests</returns>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(IEnumerable<MaintenanceRequestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MaintenanceRequestDto>>> GetPendingMaintenances(CancellationToken cancellationToken)
    {
        logger.LogDebug("Getting pending maintenance requests");
        var query = new GetPendingMaintenancesQuery();
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get maintenance request by ID
    /// </summary>
    /// <param name="id">Maintenance request ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Maintenance request details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MaintenanceRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MaintenanceRequestDto>> GetMaintenanceById(Guid id, CancellationToken cancellationToken)
    {
        logger.LogDebug("Getting maintenance request by ID: {MaintenanceId}", id);
        var query = new GetMaintenanceByIdQuery { MaintenanceRequestId = id };
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get maintenance history for a specific equipment
    /// </summary>
    /// <param name="equipmentId">Equipment ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of maintenance requests for the equipment</returns>
    [HttpGet("equipment/{equipmentId}")]
    [ProducesResponseType(typeof(GetMaintenanceRequestsQueryResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetMaintenanceRequestsQueryResult>> GetMaintenancesByEquipment(
        Guid equipmentId, 
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Getting maintenance requests for equipment: {EquipmentId}", equipmentId);
        var query = new GetMaintenanceRequestsQuery { EquipmentId = equipmentId, PageSize = 100 };
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get work queue for a specific technician
    /// </summary>
    /// <param name="technicianId">Technician ID</param>
    /// <param name="activeOnly">Show only active (Pending + InProgress) requests (default: true)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of maintenance requests assigned to technician</returns>
    [HttpGet("technician/{technicianId}")]
    [ProducesResponseType(typeof(IEnumerable<MaintenanceRequestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MaintenanceRequestDto>>> GetMaintenancesByTechnician(
        string technicianId,
        [FromQuery] bool activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting maintenance requests for technician: {TechnicianId}, ActiveOnly: {ActiveOnly}", technicianId, activeOnly);
        var query = new GetMaintenancesByTechnicianQuery { TechnicianId = technicianId, ActiveOnly = activeOnly };
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

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
        logger.LogDebug("Creating maintenance request for EquipmentId: {EquipmentId}", command.EquipmentId);
        var id = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetMaintenanceById), new { id }, id);
    }

    /// <summary>
    /// Update basic maintenance request information
    /// </summary>
    /// <param name="id">Maintenance request ID</param>
    /// <param name="command">Update command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMaintenance(Guid id, [FromBody] UpdateMaintenanceCommand command, CancellationToken cancellationToken)
    {
        if (id != command.MaintenanceRequestId)
        {
            return BadRequest("Maintenance request ID mismatch");
        }

        logger.LogDebug("Updating maintenance request: {MaintenanceId}", id);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Assign a technician to a maintenance request
    /// </summary>
    /// <param name="id">Maintenance request ID</param>
    /// <param name="command">Assign technician command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}/assign")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignTechnician(Guid id, [FromBody] AssignTechnicianCommand command, CancellationToken cancellationToken)
    {
        if (id != command.MaintenanceRequestId)
        {
            return BadRequest("Maintenance request ID mismatch");
        }

        logger.LogDebug("Assigning technician {TechnicianId} to maintenance: {MaintenanceId}", command.TechnicianId, id);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Start maintenance work
    /// </summary>
    /// <param name="id">Maintenance request ID</param>
    /// <param name="command">Start maintenance command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}/start")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartMaintenance(Guid id, [FromBody] StartMaintenanceCommand command, CancellationToken cancellationToken)
    {
        if (id != command.MaintenanceRequestId)
        {
            return BadRequest("Maintenance request ID mismatch");
        }

        logger.LogDebug("Starting maintenance: {MaintenanceId}", id);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Complete maintenance work
    /// </summary>
    /// <param name="id">Maintenance request ID</param>
    /// <param name="command">Complete maintenance command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteMaintenance(Guid id, [FromBody] CompleteMaintenanceCommand command, CancellationToken cancellationToken)
    {
        if (id != command.MaintenanceRequestId)
        {
            return BadRequest("Maintenance request ID mismatch");
        }

        logger.LogDebug("Completing maintenance: {MaintenanceId}", id);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Cancel a maintenance request
    /// </summary>
    /// <param name="id">Maintenance request ID</param>
    /// <param name="command">Cancel maintenance command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelMaintenance(Guid id, [FromBody] CancelMaintenanceCommand command, CancellationToken cancellationToken)
    {
        if (id != command.MaintenanceRequestId)
        {
            return BadRequest("Maintenance request ID mismatch");
        }

        logger.LogDebug("Cancelling maintenance: {MaintenanceId}", id);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
