using Inventory.Domain.AggregatesModel.BrandAggregate;
using MediatR;

namespace Inventory.Application.Features.Brands.Queries.GetBrand;

public record GetBrandQuery : IRequest<Brand>
{
    public Guid Id { get; set; }
}
