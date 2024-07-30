using Microsoft.AspNetCore.Mvc;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Abstractions;
using User.Application.Features.Users.Commands.SignIn;
using Microsoft.AspNetCore;
using MediatR;
using User.Application.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using static OpenIddict.Abstractions.OpenIddictConstants;
using User.Application.Features.Users.Commands.UserInfo;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using User.Application.Features.Users.Commands.SignOut;
using System.Net.Mime;

namespace User.API.Controllers;

[Route("connect")]
public class AuthorizationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthorizationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get token.
    /// </summary>
    /// <response code="200">Successful login with token response.</response>
    /// <response code="400">Invalid/Missing credentials.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("token")]
    [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(SignInResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ?? 
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        if (request.IsPasswordGrantType())
        {
            var scopes = new[] { "api" }.Intersect(request.GetScopes());
            var command = new SignInCommand
            {
                Username = request.Username,
                Password = request.Password,
                Scopes = scopes.ToList(),
            };
            var response = await _mediator.Send(command);
            return SignIn(new ClaimsPrincipal(response.Identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        else if (request.IsRefreshTokenGrantType())
        {
            var claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;

            return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        throw new BadRequestException("The specified grant type is not implemented.");
    }

    [HttpGet("userinfo")]
    [HttpPost("userinfo")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(UserInfoResponse), StatusCodes.Status200OK)]
    [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Userinfo()
    {
        var id = User.GetClaim(Claims.Subject);
        var command = new UserInfoCommand { Id = Guid.Parse(id) };
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpGet("logout")]
    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Logout()
    {
        var command = new SignOutCommand();
        var response = await _mediator.Send(command);
        return SignOut(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}
