using Inventory.Domain.AggregatesModel.VariantAggreate;
using MediatR;

namespace Inventory.Application.Features.Variants.Queries.GetVariant;

public class GetVariantQuery : IRequest<Variant>
{
    public Guid Id { get; set; }
}
