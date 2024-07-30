using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservation.Domain.AggregatesModel.BookingAggregate.Events;

public class BookingCompletedDomainEvent : INotification
{
    public Booking Booking { get; private set; }

    public BookingCompletedDomainEvent(Booking booking)
    {
        Booking = booking;
    }
}
