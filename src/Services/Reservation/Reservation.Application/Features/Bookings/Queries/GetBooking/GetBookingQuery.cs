using MediatR;
using Reservation.Domain.AggregatesModel.BookingAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservation.Application.Features.Bookings.Queries.GetBooking;

public record GetBookingQuery : IRequest<Booking>
{
    public Guid Id { get; set; }
}
