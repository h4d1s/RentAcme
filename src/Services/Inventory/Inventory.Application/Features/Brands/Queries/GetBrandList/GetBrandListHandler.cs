using AutoMapper;
using Common.Models;
using Inventory.Application.Models;
using Inventory.Application.Specifications.Brands;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using Inventory.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Brands.Queries.GetBrandList
{
    public class GetBrandListHandler : IRequestHandler<GetBrandListQuery, PagedResponse<Brand>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetBrandListHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<Brand>> Handle(GetBrandListQuery request, CancellationToken cancellationToken)
        {
            var specification = new BrandListPaginatedSpecification(
                request.Page,
                request.PageSize,
                request.Order,
                request.OrderBy);
            var brandList = await _unitOfWork.BrandRepository.ListAsync(specification);

            specification = new BrandListPaginatedSpecification(
                null,
                null,
                request.Order,
                request.OrderBy);
            var brandListAllCount = await _unitOfWork.BrandRepository.CountAsync(specification);

            return new PagedResponse<Brand>(
                request.Page,
                request.PageSize,
                brandListAllCount,
                brandList
            );
        }
    }
}
