using EquipmentManagement.Application.Features.Liquidations.Commands.CreateLiquidationRequest;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Liquidations")]
public class LiquidationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LiquidationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create liquidation request
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateLiquidationRequest(
        [FromBody] CreateLiquidationRequestCommand command,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(CreateLiquidationRequest), new { id }, id);
    }
}
