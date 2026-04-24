using Inventory.Application.Models;
using MediatR;

namespace Inventory.Application.Features.Vehicles.Queries.GetVehicle;

public record GetVehicleQuery : IRequest<VehicleResponse>
{
    public Guid Id { get; set; }
}
