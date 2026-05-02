using Asp.Versioning;
using Common.Models;
using Identity.Models;
using Inventory.Application.Features.Variants.Commands.CreateVariant;
using Inventory.Application.Features.Variants.Commands.DeleteVariant;
using Inventory.Application.Features.Variants.Commands.UpdateVariant;
using Inventory.Application.Features.Variants.Queries.GetVariant;
using Inventory.Application.Features.Variants.Queries.GetVariantList;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiVersion(1)]
[Route("api/v{v:apiVersion}/variants")]
[ApiController]
public class VariantController : ControllerBase
{
    private readonly IMediator _mediator;

    public VariantController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST api/variants
    [HttpPost]
    [Authorize(Policy = Permissions.Variants.Create)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateVariantCommand command)
    {
        var response = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id = response });
    }

    // GET api/variants/{id}
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetVariantQuery { Id = id };
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    // GET api/variants
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<Variant>>> Get([FromQuery] GetVariantListQuery query)
    {
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    // PUT api/variants
    [HttpPut("{id}")]
    [Authorize(Policy = $"{Permissions.Variants.UpdateOwn},{Permissions.Variants.UpdateAny}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(Guid id, [FromBody] UpdateVariantCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch between URL and request body.");
        }

        await _mediator.Send(command);
        return NoContent();
    }

    // DELETE api/variants/{id}
    [HttpDelete("{id}")]
    [Authorize(Policy = $"{Permissions.Variants.DeleteOwn},{Permissions.Variants.DeleteAny}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteVariantCommand { Id = id };
        await _mediator.Send(command);
        return NoContent();
    }
}
