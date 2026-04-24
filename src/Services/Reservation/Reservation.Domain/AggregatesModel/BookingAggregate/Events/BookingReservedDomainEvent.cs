using MediatR;

namespace Reservation.Domain.AggregatesModel.BookingAggregate.Events;

public class BookingReservedDomainEvent : INotification
{
    public Booking Booking { get; private set; }

    public BookingReservedDomainEvent(Booking booking)
    {
        Booking = booking;
    }
}
