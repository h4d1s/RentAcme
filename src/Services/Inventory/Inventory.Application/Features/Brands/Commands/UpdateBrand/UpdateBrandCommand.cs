using MediatR;

namespace Inventory.Application.Features.Brands.Commands.UpdateBrand;

public record UpdateBrandCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
