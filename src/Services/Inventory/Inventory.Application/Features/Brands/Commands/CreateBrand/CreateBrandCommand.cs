using MediatR;

namespace Inventory.Application.Features.Brands.Commands.CreateBrand;

public record CreateBrandCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
}
