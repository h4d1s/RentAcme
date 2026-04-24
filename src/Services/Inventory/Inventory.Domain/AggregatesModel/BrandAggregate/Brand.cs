using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.Common;
using Inventory.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace Inventory.Domain.AggregatesModel.BrandAggregate;

public class Brand
    : Entity, IAggregateRoot
{
    public string Name { get; private set; }

    private readonly List<Model> _models;
    [JsonIgnore]
    public IReadOnlyCollection<Model> Models => _models.AsReadOnly();

    public Brand(string name)
    {
        ValidateName(name);

        Name = name;
        _models = new List<Model>();
    }

    public void UpdateName(string newName)
    {
        ValidateName(newName);
        if (Name != newName)
        {
            Name = newName;
        }
    }

    public void AddModel(string name, int yearOfProduction, int numberOfSeats, Category category)
    {
        var model = new Model(name, yearOfProduction, numberOfSeats, category, Id);
        _models.Add(model);
    }

    public void RemoveModel(Guid id)
    {
        var model = _models.SingleOrDefault(m => m.Id == id);

        if (model is null)
        {
            throw new InventoryDomainException("Model not found.");
        }

        _models.Remove(model);
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InventoryDomainException("The brand name cannot be empty.");
        }
    }
}
