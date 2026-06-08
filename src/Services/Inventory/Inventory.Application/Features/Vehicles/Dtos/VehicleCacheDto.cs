using Inventory.Application.Features.Variants.Dtos;
using Inventory.Domain.AggregatesModel.VehicleAggregate;

namespace Inventory.Application.Features.Vehicles.Dtos;

public class VehicleCacheDto
{
    public Guid Id { get; set; }
    public string RegistrationPlates { get; set; } = string.Empty;
    public decimal RentalPricePerDay { get; set; }
    public int MileageKm { get; set; }
    public VehicleStatus Status { get; set; }

    public VariantCacheDto Variant { get; set; } = null!;
}
