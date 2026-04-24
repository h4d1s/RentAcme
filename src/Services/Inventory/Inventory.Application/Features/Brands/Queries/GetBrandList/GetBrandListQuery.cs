using Common.Models;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using MediatR;

namespace Inventory.Application.Features.Brands.Queries.GetBrandList;

public record GetBrandListQuery : IRequest<PagedResponse<Brand>>
{
    public string OrderBy { get; set; } = string.Empty;
    public string Order { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
