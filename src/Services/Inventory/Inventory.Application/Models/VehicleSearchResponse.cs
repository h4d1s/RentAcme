using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Models;

public record VehicleSearchResponse
{
    public Guid Id { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public decimal RentalPricePerDay { get; set; }

    public Gearbox Gearbox { get; set; }
    public Category Category { get; set; }
    public int NumberOfSeats { get; set; }
    public int Power { get; set; }
}
