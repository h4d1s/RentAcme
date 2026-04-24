using Common.Models;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using MediatR;

namespace Inventory.Application.Features.Models.Queries.GetModelList;

public record GetModelListQuery : IRequest<PagedResponse<Model>>
{
    public string OrderBy { get; set; } = string.Empty;
    public string Order { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
