using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Variants.Commands.DeleteVariant;

public record DeleteVariantCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
