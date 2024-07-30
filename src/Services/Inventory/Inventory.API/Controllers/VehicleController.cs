using MediatR;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Inventory.Application.Features.Vehicles.Queries.GetVehicle;
using Inventory.Application.Models;
using Inventory.Application.Features.Vehicles.Commands.DeleteVehicle;
using Inventory.Application.Features.Vehicles.Commands.CreateVehicle;
using Inventory.Application.Features.Vehicles.Commands.UpdateVehicle;
using Inventory.Application.Features.Vehicles.Queries.GetVehicleList;
using Inventory.Application.Features.Vehicles.Queries.SearchVehicles;
using Microsoft.AspNetCore.Authorization;
using AspNet.Security.OAuth.Validation;
using OpenIddict.Validation.AspNetCore;
using Common.Models;

namespace Inventory.API.Controllers;

[ApiVersion(1)]
[Route("api/v{v:apiVersion}/vehicles")]
[ApiController]
public class VehicleController : ControllerBase
{
    private readonly IMediator _mediator;

    public VehicleController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST api/vehicles
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateVehicleCommand command)
    {
        var response = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id = response });
    }

    // GET api/vehicles/{id}
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetVehicleQuery { Id = id };
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    // GET api/vehicles
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<VehicleResponse>>> Get([FromQuery] GetVehicleListQuery query)
    {
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    // PUT api/vehicles
    [HttpPut]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(UpdateVehicleCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    // DELETE api/vehicles/{id}
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteVehicleCommand { Id = id };
        await _mediator.Send(command);
        return NoContent();
    }

    // GET api/vehicles/search
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<VehicleSearchResponse>>> Get([FromQuery] SearchVehiclesQuery query)
    {
        var response = await _mediator.Send(query);
        return Ok(response);
    }

}
