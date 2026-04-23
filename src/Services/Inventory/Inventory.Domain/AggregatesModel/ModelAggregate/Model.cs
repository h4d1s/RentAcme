using Inventory.Domain.AggregatesModel.BrandAggregate;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Domain.Common;
using Inventory.Domain.Exceptions;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Linq;

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
        ValidateName(name);
        ValidateProductionYear(yearOfProduction);
        ValidateSeats(numberOfSeats);

        Name = name;
        YearOfProduction = yearOfProduction;
        NumberOfSeats = numberOfSeats;
        Category = category;
        BrandId = brandId;
    }

    public void UpdateName(string newName)
    {
        ValidateName(newName);

        if (Name != newName)
        {
            Name = newName;
        }
    }

    public void UpdateYearOfProduction(int yearOfProduction)
    {
        ValidateProductionYear(yearOfProduction);

        YearOfProduction = yearOfProduction;
    }

    public void UpdateNumberOfSeats(int numberOfSeats)
    {
        ValidateSeats(numberOfSeats);

        NumberOfSeats = numberOfSeats;
    }

    public void UpdateCategory(Category category)
    {
        Category = category;
    }

    public void UpdateBrandId(Guid brandId)
    {
        ValidateBrandId(brandId);

        BrandId = brandId;
    }

    public void AddVariant(string name, Gearbox gearbox, FuelType fuelType, int power, int engineSize)
    {
        if (_variants.Any(v => v.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InventoryDomainException($"Variant with name {name} already exists in this model.");
        }

        var variant = new Variant(name, gearbox, fuelType, power, engineSize, Id);
        _variants.Add(variant);
    }

    public void RemoveVariant(Guid id)
    {
        var variant = _variants.SingleOrDefault(v => v.Id == id);
        if (variant is null)
        {
            throw new InventoryDomainException("Variant not found.");
        }

        _variants.Remove(variant);
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InventoryDomainException("Model name cannot be empty.");
        }
    }

    private void ValidateProductionYear(int year)
    {
        if (year < 1900 || year > DateTime.UtcNow.Year + 1)
        {
            throw new InventoryDomainException($"Invalid production year: {year}");
        }
    }

    private void ValidateSeats(int seats)
    {
        if (seats <= 0)
        {
            throw new InventoryDomainException("Number of seats must be at least 1.");
        }
    }

    private void ValidateBrandId(Guid brandId)
    {
        if (brandId == Guid.Empty)
        {
            throw new InventoryDomainException("Brand ID cannot be empty.");
        }
    }
}
