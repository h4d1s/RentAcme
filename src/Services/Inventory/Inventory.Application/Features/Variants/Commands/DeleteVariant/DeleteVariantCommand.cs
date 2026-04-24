using MediatR;

namespace Inventory.Application.Features.Variants.Commands.DeleteVariant;

public record DeleteVariantCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
