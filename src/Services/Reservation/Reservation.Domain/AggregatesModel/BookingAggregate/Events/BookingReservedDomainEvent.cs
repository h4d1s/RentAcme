using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservation.Domain.AggregatesModel.BookingAggregate.Events;

public class BookingReservedDomainEvent : INotification
{
    public Booking Booking { get; private set; }

    public BookingReservedDomainEvent(Booking booking)
    {
        Booking = booking;
    }
}
