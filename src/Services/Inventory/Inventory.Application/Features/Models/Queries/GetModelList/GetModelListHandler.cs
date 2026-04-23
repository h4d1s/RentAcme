using AutoMapper;
using Common.Models;
using Inventory.Application.Models;
using Inventory.Application.Specifications.Models;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Models.Queries.GetModelList;

public class GetModelListHandler : IRequestHandler<GetModelListQuery, PagedResponse<Model>>
{
    private readonly IModelRepository _modelRepository;
    private readonly IMapper _mapper;

    public GetModelListHandler(
        IModelRepository modelRepository,
        IMapper mapper)
    {
        _modelRepository = modelRepository;
        _mapper = mapper;
    }

    public async Task<PagedResponse<Model>> Handle(GetModelListQuery request, CancellationToken cancellationToken)
    {
        var specification = new ModelListPaginatedSpecification(
            request.Page,
            request.PageSize, 
            request.Order,
            request.OrderBy);
        var modelList = await _modelRepository.ListAsync(specification);

        specification = new ModelListPaginatedSpecification(
            null,
            null,
            request.Order,
            request.OrderBy);
        var modelListAllCount = await _modelRepository.CountAsync(specification);

        return new PagedResponse<Model>(
            request.Page,
            request.PageSize,
            modelListAllCount,
            modelList
        );
    }
}
