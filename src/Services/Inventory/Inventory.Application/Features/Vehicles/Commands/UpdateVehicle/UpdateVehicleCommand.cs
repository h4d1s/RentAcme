using Inventory.Domain.AggregatesModel.VehicleAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Vehicles.Commands.UpdateVehicle;

public record UpdateVehicleCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public decimal RentalPricePerDay { get; set; }
    public Guid VariantId { get; set; }
}
