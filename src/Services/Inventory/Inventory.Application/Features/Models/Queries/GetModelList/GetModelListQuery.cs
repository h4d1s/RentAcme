using Common.Models;
using Inventory.Application.Models;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Models.Queries.GetModelList;

public record GetModelListQuery : IRequest<PagedResponse<Model>>
{
    public string OrderBy { get; set; } = string.Empty;
    public string Order { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
