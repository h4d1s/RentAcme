using MediatR;

namespace Reservation.Domain.AggregatesModel.BookingAggregate.Events;

public class BookingCanceledDomainEvent : INotification
{
    public Booking Booking { get; private set; }

    public BookingCanceledDomainEvent(Booking booking)
    {
        Booking = booking;
    }
}
