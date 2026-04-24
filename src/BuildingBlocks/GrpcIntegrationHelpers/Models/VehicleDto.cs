namespace GrpcIntegrationHelpers.Models;

public class VehicleDto
{
    public decimal RentalPricePerDay { get; set; }
    public string RegistrationPlates { get; set; } = string.Empty;
    public Guid VariantId { get; set; }
}
