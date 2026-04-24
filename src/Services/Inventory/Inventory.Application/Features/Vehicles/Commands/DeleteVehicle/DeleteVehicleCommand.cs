using MediatR;

namespace Inventory.Application.Features.Vehicles.Commands.DeleteVehicle;

public record DeleteVehicleCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
