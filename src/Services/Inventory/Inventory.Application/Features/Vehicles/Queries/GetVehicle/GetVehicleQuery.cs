using Inventory.Application.Models.Vehicles;
using MediatR;

namespace Inventory.Application.Features.Vehicles.Queries.GetVehicle;

public record GetVehicleQuery : IRequest<VehicleResponse>
{
    public Guid Id { get; set; }
}
