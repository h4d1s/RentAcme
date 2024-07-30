using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Inventory.Domain.AggregatesModel.BookingAggregate;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Domain.AggregatesModel.VehicleAggregate.Events;
using Inventory.Domain.Common;
using Inventory.Domain.Exceptions;

namespace Inventory.Domain.AggregatesModel.VehicleAggregate;

public class Vehicle
    : Entity, IAggregateRoot
{
    public decimal RentalPricePerDay { get; private set; }
    public string RegistrationPlates { get; private set; } = string.Empty;

    public Guid VariantId { get; private set; }
    public Variant Variant { get; } = null!;

    private readonly List<Booking> _bookings;
    [JsonIgnore]
    public IReadOnlyCollection<Booking> Bookings => _bookings.AsReadOnly();

    protected Vehicle()
    {
        _bookings = new List<Booking>();
    }

    public Vehicle(
        decimal rentalPricePerDay,
        string registrationPlates,
        Guid variantId) : this()
    {
        if (rentalPricePerDay < 0)
        {
            throw new InventoryDomainException($"{nameof(rentalPricePerDay)} must be grater than 0");
        }

        RentalPricePerDay = rentalPricePerDay;

        if (string.IsNullOrEmpty(registrationPlates))
        {
            throw new InventoryDomainException($"{nameof(registrationPlates)} must not be empty.");
        }

        RegistrationPlates = registrationPlates;
        VariantId = variantId;

        AddDomainEvent(new VehicleCreatedDomainEvent(this));
    }

    public void AddBooking(BookingStatus status, DateTime? PickupDate, DateTime? ReturnDate)
    {
        var booking = new Booking(status, Id);
        booking.SetPickupDate(PickupDate);
        booking.SetReturnDate(ReturnDate);
        _bookings.Add(booking);
    }

    public void RemoveReservation(Guid id)
    {
        var booking = _bookings.SingleOrDefault(r => r.Id == id);

        if (booking == null)
        {
            throw new InventoryDomainException("Booking not found.");
        }

        _bookings.Remove(booking);
    }
}
