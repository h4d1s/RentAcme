using AutoMapper;
using Common.Models;
using Inventory.Application.Specifications.Brands;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using MediatR;

namespace Inventory.Application.Features.Brands.Queries.GetBrandList;

public class GetBrandListHandler : IRequestHandler<GetBrandListQuery, PagedResponse<Brand>>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IMapper _mapper;

    public GetBrandListHandler(
        IBrandRepository brandRepository,
        IMapper mapper)
    {
        _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PagedResponse<Brand>> Handle(GetBrandListQuery request, CancellationToken cancellationToken)
    {
        var specification = new BrandListPaginatedSpecification(
            request.Page,
            request.PageSize,
            request.Order,
            request.OrderBy);
        var brandList = await _brandRepository.ListAsync(specification);

        specification = new BrandListPaginatedSpecification(
            null,
            null,
            request.Order,
            request.OrderBy);
        var brandListAllCount = await _brandRepository.CountAsync(specification);

        return new PagedResponse<Brand>(
            request.Page,
            request.PageSize,
            brandListAllCount,
            brandList
        );
    }
}
