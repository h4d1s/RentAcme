using Common.Models;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using Inventory.Domain.Specifications.Brands;
using MediatR;

namespace Inventory.Application.Features.Brands.Queries.GetBrandList;

public class GetBrandListHandler : IRequestHandler<GetBrandListQuery, PagedResponse<Brand>>
{
    private readonly IBrandRepository _brandRepository;

    public GetBrandListHandler(
        IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
    }

    public async Task<PagedResponse<Brand>> Handle(GetBrandListQuery request, CancellationToken cancellationToken)
    {
        var specification = new BrandListPaginatedSpecification(
            request.Page,
            request.PageSize,
            request.Order,
            request.OrderBy);
        var brandList = await _brandRepository.ListAsync(specification);

        var countSpecification = new BrandListCountSpecification(
            request.Page,
            request.PageSize,
            request.Order,
            request.OrderBy);
        var brandListAllCount = await _brandRepository.CountAsync(countSpecification);

        return new PagedResponse<Brand>(
            request.Page,
            request.PageSize,
            brandListAllCount,
            brandList
        );
    }
}
