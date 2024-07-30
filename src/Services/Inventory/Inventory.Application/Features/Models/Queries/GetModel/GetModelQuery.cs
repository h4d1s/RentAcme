using Inventory.Domain.AggregatesModel.ModelAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Models.Queries.GetModel;

public record GetModelQuery : IRequest<Model>
{
    public Guid Id { get; set; }
}
