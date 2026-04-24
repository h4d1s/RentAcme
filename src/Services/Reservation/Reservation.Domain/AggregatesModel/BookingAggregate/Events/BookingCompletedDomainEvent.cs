using MediatR;

namespace Reservation.Domain.AggregatesModel.BookingAggregate.Events;

public class BookingCompletedDomainEvent : INotification
{
    public Booking Booking { get; private set; }

    public BookingCompletedDomainEvent(Booking booking)
    {
        Booking = booking;
    }
}
