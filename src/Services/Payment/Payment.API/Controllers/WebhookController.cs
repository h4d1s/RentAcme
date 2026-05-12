using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Features.Command.StripeWebhook;

namespace Payment.API.Controllers;

[ApiVersion(1)]
[Route("api/v{v:apiVersion}/webhooks")]
[ApiController]
[AllowAnonymous]
public class WebhookController : ControllerBase
{
    private readonly IMediator _mediator;

    public WebhookController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST /stripe
    [HttpPost("stripe")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();
        var stripeSignature = Request.Headers["Stripe-Signature"];
        
        var command = new StripeWebhookCommand {
            Json = json,
            StripeSignature = stripeSignature.ToString()
        };
        await _mediator.Send(command);
        return Ok();
    }
}
