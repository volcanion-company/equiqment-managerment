using EquipmentManagement.Application.Features.Audits.Commands.BatchCreateAuditRecords;
using EquipmentManagement.Application.Features.Audits.Commands.CreateAuditRecord;
using EquipmentManagement.Application.Features.Audits.Commands.UpdateAuditRecord;
using EquipmentManagement.Application.Features.Audits.DTOs;
using EquipmentManagement.Application.Features.Audits.Queries.GetAuditById;
using EquipmentManagement.Application.Features.Audits.Queries.GetAuditRecords;
using EquipmentManagement.Application.Features.Audits.Queries.GetAuditsByEquipment;
using EquipmentManagement.Application.Features.Audits.Queries.GetAuditsForSync;
using EquipmentManagement.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManagement.WebAPI.Controllers;

/// <summary>
/// API controller that manages audit records for equipment inventory checks, including mobile app batch uploads and offline sync support.
/// </summary>
/// <param name="mediator">The mediator used to send commands and queries to the application's business logic layer.</param>
/// <param name="logger">The logger instance used to record diagnostic and operational information for the controller.</param>
[ApiController]
[Route("api/[controller]")]
[Tags("Audits")]
public class AuditsController(IMediator mediator, ILogger<AuditsController> logger) : ControllerBase
{
    /// <summary>
    /// Retrieves a paginated list of audit records with optional filtering by equipment, result, and date range.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve (default: 1).</param>
    /// <param name="pageSize">The number of items per page (default: 10, max: 100).</param>
    /// <param name="equipmentId">Optional equipment ID to filter audit records.</param>
    /// <param name="result">Optional audit result filter (Match, NotMatch, Missing).</param>
    /// <param name="fromDate">Optional start date for date range filter.</param>
    /// <param name="toDate">Optional end date for date range filter.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An HTTP 200 OK response containing paginated audit records.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetAuditRecordsQueryResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetAuditRecordsQueryResult>> GetAuditRecords(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? equipmentId = null,
        [FromQuery] AuditResult? result = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAuditRecordsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            EquipmentId = equipmentId,
            Result = result,
            FromDate = fromDate,
            ToDate = toDate
        };

        var auditRecords = await mediator.Send(query, cancellationToken);
        return Ok(auditRecords);
    }

    /// <summary>
    /// Retrieves detailed information about a specific audit record by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the audit record.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An HTTP 200 OK response containing the audit record details, or HTTP 404 Not Found if the record does not exist.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AuditRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AuditRecordDto>> GetAuditById(Guid id, CancellationToken cancellationToken)
    {
        logger.LogDebug("Retrieving audit record with ID: {AuditId}", id);

        var query = new GetAuditByIdQuery { AuditRecordId = id };
        var auditRecord = await mediator.Send(query, cancellationToken);

        return Ok(auditRecord);
    }

    /// <summary>
    /// Retrieves the complete audit history for a specific equipment.
    /// </summary>
    /// <param name="equipmentId">The unique identifier of the equipment.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An HTTP 200 OK response containing all audit records for the specified equipment.</returns>
    [HttpGet("equipment/{equipmentId}")]
    [ProducesResponseType(typeof(IEnumerable<AuditRecordDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AuditRecordDto>>> GetAuditsByEquipment(Guid equipmentId, CancellationToken cancellationToken)
    {
        logger.LogDebug("Retrieving audit history for equipment: {EquipmentId}", equipmentId);

        var query = new GetAuditsByEquipmentQuery { EquipmentId = equipmentId };
        var auditRecords = await mediator.Send(query, cancellationToken);

        return Ok(auditRecords);
    }

    /// <summary>
    /// Retrieves audit records for mobile app offline synchronization. Returns records modified after the specified date.
    /// </summary>
    /// <param name="sinceDate">Optional datetime to retrieve only records modified after this date (for incremental sync).</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An HTTP 200 OK response containing audit records for synchronization.</returns>
    [HttpGet("sync")]
    [ProducesResponseType(typeof(IEnumerable<AuditRecordDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AuditRecordDto>>> GetAuditsForSync(
        [FromQuery] DateTime? sinceDate = null,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Retrieving audit records for sync since: {SinceDate}", sinceDate);

        var query = new GetAuditsForSyncQuery { SinceDate = sinceDate };
        var auditRecords = await mediator.Send(query, cancellationToken);

        return Ok(auditRecords);
    }

    /// <summary>
    /// Creates a new audit record and returns the unique identifier of the created record.
    /// </summary>
    /// <param name="command">The command containing the details required to create the audit record. Cannot be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An HTTP 201 Created response containing the unique identifier of the newly created audit record. Returns an HTTP
    /// 400 Bad Request response if the input is invalid.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateAuditRecord([FromBody] CreateAuditRecordCommand command, CancellationToken cancellationToken)
    {
        logger.LogDebug("Creating a new audit record for EquipmentId: {EquipmentId}", command.EquipmentId);

        var id = await mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetAuditById), new { id }, id);
    }

    /// <summary>
    /// Creates multiple audit records in a single batch operation for mobile app offline upload. Supports up to 1000 records per request.
    /// </summary>
    /// <param name="command">The command containing an array of audit records to create.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An HTTP 200 OK response containing the batch operation result with success/failure counts and any errors encountered.</returns>
    [HttpPost("batch")]
    [ProducesResponseType(typeof(BatchCreateAuditRecordsResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BatchCreateAuditRecordsResult>> BatchCreateAuditRecords(
        [FromBody] BatchCreateAuditRecordsCommand command,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing batch audit creation with {Count} records", command.AuditRecords.Count);

        var result = await mediator.Send(command, cancellationToken);

        logger.LogInformation(
            "Batch audit creation completed: {SuccessCount} succeeded, {FailureCount} failed",
            result.SuccessCount, result.FailureCount);

        return Ok(result);
    }

    /// <summary>
    /// Updates an existing audit record. Only the result, note, and location fields can be modified.
    /// </summary>
    /// <param name="id">The unique identifier of the audit record to update.</param>
    /// <param name="command">The command containing the updated audit record information.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An HTTP 204 No Content response if successful, or HTTP 404 Not Found if the record does not exist.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAuditRecord(Guid id, [FromBody] UpdateAuditRecordCommand command, CancellationToken cancellationToken)
    {
        logger.LogDebug("Updating audit record: {AuditId}", id);

        command.AuditRecordId = id;
        await mediator.Send(command, cancellationToken);

        return NoContent();
    }
}
