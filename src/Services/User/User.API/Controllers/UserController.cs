using Microsoft.AspNetCore.Mvc;
using MediatR;
using Asp.Versioning;
using MassTransit.Transports;
using User.Application.Features.Users.Commands.SignIn;
using User.Application.Features.Users.Commands.SignUp;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore;

namespace User.API.Controllers;

/// <summary>
/// User controller.
/// </summary>
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="command">Register command with user details.</param>
    /// <response code="201">Successfully created with registration response.</response>
    /// <response code="400">Invalid/Missing registration data.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("signup")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SignUp([FromBody] SignUpCommand command)
    {
        var response = await _mediator.Send(command);
        return Created();
    }
}
