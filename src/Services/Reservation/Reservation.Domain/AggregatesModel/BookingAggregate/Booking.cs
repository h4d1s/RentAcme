using Catalog.Domain.Common;
using Reservation.Domain.AggregatesModel.BookingAggregate.Events;
using Reservation.Domain.Common;
using Reservation.Domain.Exceptions;

namespace Reservation.Domain.AggregatesModel.BookingAggregate;

public class Booking
    : Entity, IAggregateRoot
{
    public Guid VehicleId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime PickupDate { get; private set; }
    public DateTime ReturnDate { get; private set; }
    public BookingStatus Status { get; private set; }
    public DateTime? BookingDate { get; private set; }
    public decimal Price { get; private set; }

    protected Booking()
    {
    }

    public Booking(string vehicleId, string userId, DateTime pickupDate, DateTime returnDate) : this()
    {
        VehicleId = Guid.Parse(vehicleId);
        UserId = Guid.Parse(userId);
        PickupDate = pickupDate;
        ReturnDate = returnDate;
        Status = BookingStatus.Reserved;
        BookingDate = DateTime.UtcNow;

        AddDomainEvent(new BookingReservedDomainEvent(this));
    }

    public void SetPrice(decimal rentalPricePerDay)
    {
        var days = (ReturnDate.Date - PickupDate.Date).Days + 1;
        Price = days * rentalPricePerDay;
    }

    public void SetCompleteStatus()
    {
        if (Status != BookingStatus.Reserved)
        {
            StatusChangeException(BookingStatus.Completed);
        }

        Status = BookingStatus.Completed;
        AddDomainEvent(new BookingCompletedDomainEvent(this));
    }

    public void SetCanceledStatus()
    {
        if (Status != BookingStatus.Reserved)
        {
            StatusChangeException(BookingStatus.Canceled);
        }

        Status = BookingStatus.Canceled;
        AddDomainEvent(new BookingCanceledDomainEvent(this));
    }

    private void StatusChangeException(BookingStatus bookingStatusToChange)
    {
        throw new ReservationDomainException($"Is not possible to change the order status from {Status} to {bookingStatusToChange}.");
    }
}
