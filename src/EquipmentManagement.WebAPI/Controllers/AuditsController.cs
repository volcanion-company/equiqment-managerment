using EquipmentManagement.Application.Features.Audits.Commands.CreateAuditRecord;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Audits")]
public class AuditsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuditsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create audit record (for mobile app QR scanning)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateAuditRecord(
        [FromBody] CreateAuditRecordCommand command,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(CreateAuditRecord), new { id }, id);
    }
}
