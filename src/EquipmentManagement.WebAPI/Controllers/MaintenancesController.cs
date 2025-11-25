using EquipmentManagement.Application.Features.Maintenances.Commands.CreateMaintenanceRequest;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Maintenances")]
public class MaintenancesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MaintenancesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create maintenance request
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateMaintenanceRequest(
        [FromBody] CreateMaintenanceRequestCommand command,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(CreateMaintenanceRequest), new { id }, id);
    }
}
