using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Inventory.Domain.Common;
using Inventory.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace Inventory.Domain.AggregatesModel.VariantAggreate;

public class Variant
    : Entity, IAggregateRoot
{
    public string Name { get; private set; } = "Default";
    public Gearbox Gearbox { get; private set; }
    public FuelType FuelType { get; private set; }
    public int Power { get; private set; }
    public int EngineSize { get; private set; }

    public Guid ModelId { get; private set; }
    [JsonIgnore]
    public Model Model { get; private set; } = null!;

    [JsonIgnore]
    public Vehicle? Vehicle { get; private set; }

    public Variant(string name, Gearbox gearbox, FuelType fuelType, int power, int engineSize, Guid modelId)
    {
        ValidateName(name);
        ValidatePower(power);
        ValidateEngineSize(engineSize);
        ValidateModelId(modelId);

        Name = name;
        Gearbox = gearbox;
        FuelType = fuelType;
        Power = power;
        EngineSize = engineSize;
        ModelId = modelId;
    }

    public void UpdateName(string name)
    {
        ValidateName(name);
        Name = name;
    }

    public void UpdateGearbox(Gearbox gearbox)
    {
        Gearbox = gearbox;
    }

    public void UpdateFuelType(FuelType fuelType)
    {
        FuelType = fuelType;
    }

    public void UpdatePower(int power)
    {
        ValidatePower(power);
        Power = power;
    }

    public void UpdateEngineSize(int engineSize)
    {
        ValidateEngineSize(engineSize);
        EngineSize = engineSize;
    }

    public void UpdateModelId(Guid modelId)
    {
        ValidateModelId(modelId);
        ModelId = modelId;
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InventoryDomainException("Model name cannot be empty.");
        }
    }

    private void ValidatePower(int power)
    {
        if (power <= 0)
        {
            throw new InventoryDomainException("Power must be a positive integer.");
        }
    }

    private void ValidateEngineSize(int engineSize)
    {
        if (engineSize <= 0)
        {
            throw new InventoryDomainException("Engine size must be a positive integer.");
        }
    }

    private void ValidateModelId(Guid modelId)
    {
        if (modelId == Guid.Empty)
        {
            throw new InventoryDomainException("ModelId cannot be an empty Guid.");
        }
    }
}
