using Inventory.Application.Models;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Vehicles.Queries.GetVehicle;

public record GetVehicleQuery : IRequest<VehicleResponse>
{
    public Guid Id { get; set; }
}
