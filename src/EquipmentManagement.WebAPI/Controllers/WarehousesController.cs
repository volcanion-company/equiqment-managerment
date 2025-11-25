using EquipmentManagement.Application.Features.Warehouses.Commands.CreateTransaction;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Warehouses")]
public class WarehousesController : ControllerBase
{
    private readonly IMediator _mediator;

    public WarehousesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create warehouse transaction (import/export)
    /// </summary>
    [HttpPost("transactions")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateTransaction(
        [FromBody] CreateWarehouseTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(CreateTransaction), new { id }, id);
    }
}
