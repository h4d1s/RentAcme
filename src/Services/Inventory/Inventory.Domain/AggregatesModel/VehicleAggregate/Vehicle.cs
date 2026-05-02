using Inventory.Domain.AggregatesModel.BookingAggregate;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Domain.AggregatesModel.VehicleAggregate.Events;
using Inventory.Domain.Common;
using Inventory.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace Inventory.Domain.AggregatesModel.VehicleAggregate;

public class Vehicle
    : Entity, IAggregateRoot
{
    public decimal RentalPricePerDay { get; private set; }
    public string RegistrationPlates { get; private set; } = string.Empty;

    public Guid VariantId { get; private set; }
    [JsonIgnore]
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
        ValidateRentalPrice(rentalPricePerDay);
        ValidateRegistrationPlates(registrationPlates);
        ValidateVariantId(variantId);

        RentalPricePerDay = rentalPricePerDay;
        RegistrationPlates = registrationPlates;
        VariantId = variantId;

        AddDomainEvent(new VehicleCreatedDomainEvent(this));
    }

    public void UpdateRentalPrice(decimal price)
    {
        ValidateRentalPrice(price);
        RentalPricePerDay = price;
    }

    public void UpdateRegistrationPlates(string plates)
    {
        ValidateRegistrationPlates(plates);
        RegistrationPlates = plates;
    }

    public void UpdateVariantId(Guid variantId)
    {
        ValidateVariantId(variantId);
        VariantId = variantId;
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

        if (booking is null)
        {
            throw new InventoryDomainException("Booking not found.");
        }

        _bookings.Remove(booking);
    }

    private void ValidateRentalPrice(decimal price)
    {
        if (price <= 0)
        {
            throw new InventoryDomainException("Rental price must be greater than zero.");
        }
    }

    private void ValidateRegistrationPlates(string plates)
    {
        if (string.IsNullOrWhiteSpace(plates))
        {
            throw new InventoryDomainException("Registration plates cannot be empty.");
        }
    }

    private void ValidateVariantId(Guid variantId)
    {
        if (variantId == Guid.Empty)
        {
            throw new InventoryDomainException("VariantId must be a valid, non-empty Guid.");
        }
    }
}
