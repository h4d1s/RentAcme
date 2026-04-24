using MediatR;

namespace Inventory.Application.Features.Brands.Commands.DeleteBrand;

public record DeleteBrandCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
