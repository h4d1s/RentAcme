using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Domain.Common;
using Inventory.Domain.Exceptions;

namespace Inventory.Domain.AggregatesModel.ModelAggregate;

public class Model
    : Entity, IAggregateRoot
{
    public string Name { get; private set; } = null!;
    public int YearOfProduction { get; private set; }
    public int NumberOfSeats { get; private set; }
    public Category Category { get; private set; }

    public Guid? BrandId { get; private set; }
    public Brand Brand { get; } = null!;

    private readonly List<Variant> _variants;
    [JsonIgnore]
    public IReadOnlyCollection<Variant> Variants => _variants.AsReadOnly();

    protected Model()
    {
        _variants = new List<Variant>();
    }

    public Model(string name, int yearOfProduction, int numberOfSeats, Category category, Guid brandId) : this()
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InventoryDomainException($"Model {nameof(name)} cannot be empty.");
        }

        Name = name;

        if (yearOfProduction < 1900)
        {
            throw new InventoryDomainException($"Model {nameof(yearOfProduction)} must be grater than 1900");
        }

        if (yearOfProduction > DateTime.UtcNow.Year)
        {
            throw new InventoryDomainException($"Model {nameof(yearOfProduction)} cannot be in the future");
        }

        YearOfProduction = yearOfProduction;

        if (numberOfSeats <= 1)
        {
            throw new InventoryDomainException($"Model {nameof(numberOfSeats)} must be greater than 1");
        }

        NumberOfSeats = numberOfSeats;
        Category = category;
        BrandId = brandId;
    }

    public void AddVariant(string name, Gearbox gearbox, FuelType fuelType, int power, int engineSize)
    {
        var variant = new Variant(name, gearbox, fuelType, power, engineSize, Id);
        _variants.Add(variant);
    }

    public void RemoveVariant(Guid id)
    {
        var variant = _variants.SingleOrDefault(v => v.Id == id);
        if (variant == null)
        {
            throw new InventoryDomainException("Variant not found.");
        }

        _variants.Remove(variant);
    }
}
