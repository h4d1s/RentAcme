using Inventory.Domain.AggregatesModel.VariantAggreate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Variants.Commands.UpdateVariant;

public record UpdateVariantCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Gearbox Gearbox { get; set; }
    public FuelType FuelType { get; set; }
    public int Power { get; set; }
    public int EngineSize { get; set; }
    public Guid ModelId { get; set; }
}
