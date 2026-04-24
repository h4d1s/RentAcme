using Asp.Versioning;
using GrpcIntegrationHelpers.Models;
using Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.Application.Features.Users.Commands.DeleteUser;
using User.Application.Features.Users.Commands.UpdateUser;
using User.Application.Features.Users.Queries.GetUser;

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
    /// Get user by id.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <response code="200">Returns the user details.</response>
    /// <response code="404">User not found.</response>
    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetUserQuery { Id = id };
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    /// <summary>
    /// Update user details.
    /// </summary>
    /// <param name="id">The user ID to update.</param>
    /// <param name="command">The updated user data.</param>
    /// <response code="204">User successfully updated.</response>
    /// <response code="400">Validation error or mismatched ID.</response>
    /// <response code="404">User not found.</response>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Delete user by id.
    /// </summary>
    /// <param name="command">Delete command containing the user ID.</param>
    /// <response code="204">User successfully deleted.</response>
    /// <response code="400">Invalid or missing user ID.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteUserCommand { Id = id };
        await _mediator.Send(command);
        return NoContent();
    }
}
