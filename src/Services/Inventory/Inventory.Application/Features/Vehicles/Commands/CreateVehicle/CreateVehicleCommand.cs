using Inventory.Domain.AggregatesModel.VehicleAggregate;
using MediatR;

namespace Inventory.Application.Features.Vehicles.Commands.CreateVehicle;

public class CreateVehicleCommand : IRequest<Guid>
{
    public decimal RentalPricePerDay { get; set; }
    public string RegistrationPlates { get; set; } = string.Empty;
    public Guid VariantId { get; set; }
}
