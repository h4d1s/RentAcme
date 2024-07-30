using Inventory.Domain.AggregatesModel.BrandAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Brands.Queries.GetBrand
{
    public record GetBrandQuery : IRequest<Brand>
    {
        public Guid Id { get; set; }
    }
}
