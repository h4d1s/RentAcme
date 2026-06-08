using Inventory.Application.Features.Models.Dtos;
using Inventory.Domain.AggregatesModel.VariantAggreate;

namespace Inventory.Application.Features.Variants.Dtos;

public class VariantCacheDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Gearbox Gearbox { get; set; }
    public FuelType FuelType { get; set; }
    public int Power { get; set; }
    public int EngineSize { get; set; }

    public ModelCacheDto Model { get; set; } = null!;
}
