﻿using MediatR;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Inventory.Application.Features.Models.Commands.CreateModel;
using Inventory.Application.Features.Models.Queries.GetModel;
using Inventory.Application.Models;
using Inventory.Application.Features.Models.Commands.UpdateModel;
using Inventory.Application.Features.Models.Commands.DeleteModel;
using Inventory.Application.Features.Models.Queries.GetModelList;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using Microsoft.AspNetCore.Authorization;
using Common.Models;

namespace Inventory.API.Controllers;

[ApiVersion(1)]
[Route("api/v{v:apiVersion}/models")]
[ApiController]
public class ModelController : ControllerBase
{
    private readonly IMediator _mediator;

    public ModelController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST api/models
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateModelCommand command)
    {
        var response = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id = response });
    }

    // GET api/models/{id}
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetModelQuery { Id = id };
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    // GET api/models
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<Model>>> Get([FromQuery] GetModelListQuery query)
    {
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    // PUT api/models
    [HttpPut]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(UpdateModelCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    // DELETE api/models/{id}
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteModelCommand { Id = id };
        await _mediator.Send(command);
        return NoContent();
    }
}