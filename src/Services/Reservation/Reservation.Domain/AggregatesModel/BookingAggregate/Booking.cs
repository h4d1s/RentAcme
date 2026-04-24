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

    public Booking(Guid vehicleId, Guid userId, DateTime pickupDate, DateTime returnDate) : this()
    {
        ValidateBookingDate(pickupDate);
        if (returnDate <= pickupDate)
        {
            throw new ReservationDomainException("Return date must be later than the pickup date.");
        }

        VehicleId = vehicleId;
        UserId = userId;
        PickupDate = pickupDate;
        ReturnDate = returnDate;
        Status = BookingStatus.Reserved;
        BookingDate = DateTime.UtcNow;

        AddDomainEvent(new BookingReservedDomainEvent(this));
    }

    public void UpdatePickupDate(DateTime pickupDate)
    {
        if (pickupDate >= ReturnDate)
        {
            throw new ReservationDomainException("Pickup date must be earlier than the return date.");
        }

        PickupDate = pickupDate;
    }

    public void UpdateReturnDate(DateTime returnDate)
    {
        if (returnDate <= PickupDate)
        {
            throw new ReservationDomainException("Return date must be later than the pickup date.");
        }

        ReturnDate = returnDate;
    }

    public void UpdateBookingDate(DateTime? bookingDate)
    {
        BookingDate = bookingDate;
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

    private void ValidateBookingDate(DateTime date)
    {
        if (date < DateTime.UtcNow.AddMinutes(-1))
        {
            throw new ArgumentException("Booking date cannot be in the past.");
        }
    }
}
