using MediatR;

namespace Inventory.Application.Features.Vehicles.Commands.UpdateVehicle;

public record UpdateVehicleCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public decimal RentalPricePerDay { get; set; }
    public Guid VariantId { get; set; }
    public string RegistrationPlates { get; set; } = string.Empty;
}
