﻿using MediatR;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Inventory.Application.Features.Variants.Commands.CreateVariant;
using Inventory.Application.Features.Variants.Queries.GetVariant;
using Inventory.Application.Models;
using Inventory.Application.Features.Variants.Commands.UpdateVariant;
using Inventory.Application.Features.Variants.Commands.DeleteVariant;
using Inventory.Application.Features.Variants.Queries.GetVariantList;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Microsoft.AspNetCore.Authorization;
using Common.Models;

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
    [Authorize]
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
    [HttpPut]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(UpdateVariantCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    // DELETE api/variants/{id}
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteVariantCommand { Id = id };
        await _mediator.Send(command);
        return NoContent();
    }
}
