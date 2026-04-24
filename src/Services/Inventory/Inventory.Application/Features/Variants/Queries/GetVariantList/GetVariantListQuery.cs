using Common.Models;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using MediatR;

namespace Inventory.Application.Features.Variants.Queries.GetVariantList;

public record GetVariantListQuery : IRequest<PagedResponse<Variant>>
{
    public string OrderBy { get; set; } = string.Empty;
    public string Order { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
