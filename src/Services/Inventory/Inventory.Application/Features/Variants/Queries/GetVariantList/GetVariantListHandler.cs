using Common.Models;
using Inventory.Domain.Specifications.Models;
using Inventory.Domain.Specifications.Variants;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Caching.Services;
using MediatR;

namespace Inventory.Application.Features.Variants.Queries.GetVariantList;

public class GetVariantListHandler : IRequestHandler<GetVariantListQuery, PagedResponse<Variant>>
{
    private readonly IVariantRepository _variantRepository;
    private readonly ICacheService _cacheService;

    public GetVariantListHandler(
        IVariantRepository variantRepository,
        ICacheService cacheService)
    {
        _variantRepository = variantRepository ?? throw new ArgumentNullException(nameof(variantRepository));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    }

    public async Task<PagedResponse<Variant>> Handle(GetVariantListQuery request, CancellationToken cancellationToken)
    {
        var specification = new VariantListPaginatedSpecification(
            request.Page,
            request.PageSize,
            request.Order,
            request.OrderBy);
        var variantList = await _variantRepository.ListAsync(specification);

        var countSpecification = new VariantListCountSpecification(
            request.Page,
            request.PageSize,
            request.Order,
            request.OrderBy);
        var variantListAllCount = await _variantRepository.CountAsync(countSpecification);

        return new PagedResponse<Variant>(
            request.Page,
            request.PageSize,
            variantListAllCount,
            variantList
        );
    }
}
