using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Inventory.Domain.Common;
using Inventory.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
    public Model Model { get; private set; } = null!;

    [JsonIgnore]
    public Vehicle? Vehicle { get; private set; }

    public Variant(string name, Gearbox gearbox, FuelType fuelType, int power, int engineSize, Guid modelId)
    {
        Name = name;
        Gearbox = gearbox;
        FuelType = fuelType;

        if (power <= 0)
        {
            throw new InventoryDomainException($"{nameof(power)} must be grater than 0");
        }

        Power = power;

        if (engineSize <= 0)
        {
            throw new InventoryDomainException($"{nameof(engineSize)} must be grater than 0");
        }

        EngineSize = engineSize;
        ModelId = modelId;
    }
}
