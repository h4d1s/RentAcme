using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reservation.Application.Features.Bookings.Queries.GetBooking;
using Reservation.Application.Features.Bookings.Commands.CancelBooking;
using Reservation.Application.Features.Bookings.Commands.ReserveBooking;
using Reservation.Application.Features.Bookings.Queries.GetBookingList;
using Reservation.Domain.AggregatesModel.BookingAggregate;
using Reservation.Application.Features.Bookings.Commands.CompleteBooking;
using Microsoft.AspNetCore.Authorization;
using Common.Models;

namespace Reservation.API.Controllers;

[ApiVersion(1)]
[Route("api/v{v:apiVersion}/bookings")]
[ApiController]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public BookingController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST api/bookings
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] ReserveBookingCommand command)
    {
        var response = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id = response });
    }

    // POST api/bookings/cancel
    [HttpPost("cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CancelBookingCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    // POST api/bookings/complete
    [HttpPost("complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CompleteBookingCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    // GET api/bookings/{id}
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Booking), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetBookingQuery { Id = id };
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    // GET api/bookings
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<Booking>>> Get([FromQuery] GetBookingListQuery query)
    {
        var response = await _mediator.Send(query);
        return Ok(response);
    }
}
