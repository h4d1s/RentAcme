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
    public bool IsLocked { get; private set; }

    public Guid VariantId { get; private set; }
    [JsonIgnore]
    public Variant Variant { get; } = null!;

    protected Vehicle()
    {
    }

    public Vehicle(
        decimal rentalPricePerDay,
        string registrationPlates,
        Guid variantId,
        bool isLocked = false) : this()
    {
        ValidateRentalPrice(rentalPricePerDay);
        ValidateRegistrationPlates(registrationPlates);
        ValidateVariantId(variantId);

        RentalPricePerDay = rentalPricePerDay;
        RegistrationPlates = registrationPlates;
        VariantId = variantId;
        IsLocked = isLocked;

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

    public void UpdateIsLocked(bool isLocked)
    { 
        IsLocked = isLocked;
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
