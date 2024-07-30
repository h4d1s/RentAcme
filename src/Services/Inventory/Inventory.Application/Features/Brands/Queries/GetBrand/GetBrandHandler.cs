using AutoMapper;
using Inventory.Application.Exceptions;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using Inventory.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Brands.Queries.GetBrand
{
    public class GetBrandHandler : IRequestHandler<GetBrandQuery, Brand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetBrandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Brand> Handle(GetBrandQuery request, CancellationToken cancellationToken)
        {
            var brand = await _unitOfWork.BrandRepository.GetByIdAsync(request.Id);

            if (brand == null)
            {
                throw new NotFoundException($"Brand with Id {request.Id} not found.");
            }

            return brand;
        }
    }
}
