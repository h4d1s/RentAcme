using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Brands.Commands.CreateBrand;

public record CreateBrandCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
}
