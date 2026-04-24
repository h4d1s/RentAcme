using Inventory.Application.Exceptions;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using MediatR;

namespace Inventory.Application.Features.Brands.Queries.GetBrand;

public class GetBrandHandler : IRequestHandler<GetBrandQuery, Brand>
{
    private readonly IBrandRepository _brandRepository;

    public GetBrandHandler(
        IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
    }

    public async Task<Brand> Handle(GetBrandQuery request, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByIdAsync(request.Id);

        if (brand is null)
        {
            throw new NotFoundException($"Brand with Id {request.Id} not found.");
        }

        return brand;
    }
}
