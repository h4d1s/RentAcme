using Asp.Versioning;
using Common.Models;
using Identity.Models;
using Inventory.Application.Features.Brands.Commands.CreateBrand;
using Inventory.Application.Features.Brands.Commands.DeleteBrand;
using Inventory.Application.Features.Brands.Commands.UpdateBrand;
using Inventory.Application.Features.Brands.Queries.GetBrand;
using Inventory.Application.Features.Brands.Queries.GetBrandList;
using Inventory.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiVersion(1)]
[Route("api/v{v:apiVersion}/brands")]
[ApiController]
public class BrandController : ControllerBase
{
    private readonly IMediator _mediator;

    public BrandController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST api/brands
    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateBrandCommand command)
    {
        var response = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id = response });
    }

    // GET api/brands/{id}
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetBrandQuery { Id = id };
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    // GET api/brands
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<VehicleResponse>>> Get([FromQuery] GetBrandListQuery query)
    {
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    // PUT api/brands/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(Guid id, [FromBody] UpdateBrandCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch between URL and request body.");
        }

        await _mediator.Send(command);
        return NoContent();
    }

    // DELETE api/brands/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteBrandCommand { Id = id };
        await _mediator.Send(command);
        return NoContent();
    }
}
