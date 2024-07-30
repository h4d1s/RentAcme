using AutoMapper;
using Common.Models;
using Inventory.Application.Models;
using Inventory.Application.Specifications.Models;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Variants.Queries.GetVariantList;

public class GetVariantListHandler : IRequestHandler<GetVariantListQuery, PagedResponse<Variant>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetVariantListHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResponse<Variant>> Handle(GetVariantListQuery request, CancellationToken cancellationToken)
    {
        var specification = new VariantListPaginatedSpecification(
            request.Page,
            request.PageSize,
            request.Order,
            request.OrderBy);
        var variantList = await _unitOfWork.VariantRepository.ListAsync(specification);

        specification = new VariantListPaginatedSpecification(
            null,
            null,
            request.Order,
            request.OrderBy);
        var variantListAllCount = await _unitOfWork.VariantRepository.CountAsync(specification);

        return new PagedResponse<Variant>(
            request.Page,
            request.PageSize,
            variantListAllCount,
            variantList
        );
    }
}
