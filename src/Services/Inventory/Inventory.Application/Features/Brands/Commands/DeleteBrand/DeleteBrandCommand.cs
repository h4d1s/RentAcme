using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Brands.Commands.DeleteBrand;

public record DeleteBrandCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
