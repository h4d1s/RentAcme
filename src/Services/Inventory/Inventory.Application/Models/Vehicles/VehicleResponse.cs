namespace Inventory.Application.Models.Vehicles;

public record VehicleResponse
{
    public Guid Id { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Variant { get; set; } = string.Empty;
}
