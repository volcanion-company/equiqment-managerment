using EquipmentManagement.Application.Features.Audits.Commands.CreateAuditRecord;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManagement.WebAPI.Controllers;

/// <summary>
/// API controller that manages audit records, including creation of new audit entries for mobile app QR scanning.
/// </summary>
/// <param name="mediator">The mediator used to send commands and queries to the application's business logic layer.</param>
/// <param name="logger">The logger instance used to record diagnostic and operational information for the controller.</param>
[ApiController]
[Route("api/[controller]")]
[Tags("Audits")]
public class AuditsController(IMediator mediator, ILogger<AuditsController> logger) : ControllerBase
{
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
        // Log the creation attempt
        logger.LogDebug("Creating a new audit record for EquipmentId: {EquipmentId}", command.EquipmentId);
        // Send the command to create the audit record
        var id = await mediator.Send(command, cancellationToken);
        // Return a Created response with the new record's ID
        return CreatedAtAction(nameof(CreateAuditRecord), new { id }, id);
    }
}
