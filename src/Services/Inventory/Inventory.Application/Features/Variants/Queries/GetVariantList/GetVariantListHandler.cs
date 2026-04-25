using AutoMapper;
using Common.Models;
using Inventory.Application.Specifications.Models;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using MediatR;

namespace Inventory.Application.Features.Variants.Queries.GetVariantList;

public class GetVariantListHandler : IRequestHandler<GetVariantListQuery, PagedResponse<Variant>>
{
    private readonly IVariantRepository _variantRepository;

    public GetVariantListHandler(
        IVariantRepository variantRepository)
    {
        _variantRepository = variantRepository ?? throw new ArgumentNullException(nameof(variantRepository));
    }

    public async Task<PagedResponse<Variant>> Handle(GetVariantListQuery request, CancellationToken cancellationToken)
    {
        var specification = new VariantListPaginatedSpecification(
            request.Page,
            request.PageSize,
            request.Order,
            request.OrderBy);
        var variantList = await _variantRepository.ListAsync(specification);

        specification = new VariantListPaginatedSpecification(
            null,
            null,
            request.Order,
            request.OrderBy);
        var variantListAllCount = await _variantRepository.CountAsync(specification);

        return new PagedResponse<Variant>(
            request.Page,
            request.PageSize,
            variantListAllCount,
            variantList
        );
    }
}
