using Inventory.Domain.AggregatesModel.ModelAggregate;
using MediatR;

namespace Inventory.Application.Features.Models.Queries.GetModel;

public record GetModelQuery : IRequest<Model>
{
    public Guid Id { get; set; }
}
