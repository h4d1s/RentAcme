using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Vehicles.Commands.DeleteVehicle;

public record DeleteVehicleCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
