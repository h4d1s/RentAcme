using Inventory.Domain.AggregatesModel.VariantAggreate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Variants.Queries.GetVariant;

public class GetVariantQuery : IRequest<Variant>
{
    public Guid Id { get; set; }
}
