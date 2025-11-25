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

[ApiController]
[Route("api/[controller]")]
[Tags("Equipments")]
public class EquipmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EquipmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get paginated list of equipments
    /// </summary>
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
        var query = new GetEquipmentsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Type = type,
            Status = status,
            Keyword = keyword
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get equipment by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EquipmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EquipmentDto>> GetEquipment(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetEquipmentByIdQuery { Id = id };
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Create new equipment
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateEquipment(
        [FromBody] CreateEquipmentCommand command,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetEquipment), new { id }, id);
    }

    /// <summary>
    /// Update equipment
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEquipment(
        Guid id,
        [FromBody] UpdateEquipmentCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Delete equipment (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEquipment(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteEquipmentCommand { Id = id };
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
