using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservation.Application.Features.Bookings.Commands.CompleteBooking;

public class CompleteBookingCommand : IRequest<Unit>
{
    public Guid BookingId { get; set; }
}
