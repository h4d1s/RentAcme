using Inventory.Application.Features.Brands.Dtos;
using Inventory.Domain.AggregatesModel.ModelAggregate;

namespace Inventory.Application.Features.Models.Dtos;

public class ModelCacheDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int YearOfProduction { get; set; }
    public int NumberOfSeats { get; set; }
    public Category Category { get; set; }

    public BrandCacheDto Brand { get; set; } = null!;
}
