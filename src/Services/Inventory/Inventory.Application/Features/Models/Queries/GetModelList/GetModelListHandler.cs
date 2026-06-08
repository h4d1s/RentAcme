using Common.Models;
using Inventory.Domain.Specifications.Models;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using MediatR;
using Caching.Services;

namespace Inventory.Application.Features.Models.Queries.GetModelList;

public class GetModelListHandler : IRequestHandler<GetModelListQuery, PagedResponse<Model>>
{
    private readonly IModelRepository _modelRepository;
    private readonly ICacheService _cacheService;

    public GetModelListHandler(
        IModelRepository modelRepository,
        ICacheService cacheService)
    {
        _modelRepository = modelRepository;
        _cacheService = cacheService;
    }

    public async Task<PagedResponse<Model>> Handle(GetModelListQuery request, CancellationToken cancellationToken)
    {
        var specification = new ModelListPaginatedSpecification(
            request.Page,
            request.PageSize,
            request.Order,
            request.OrderBy);
        var modelList = await _modelRepository.ListAsync(specification);

        var countSpecification = new ModelListCountSpecification(
            request.Page,
            request.PageSize,
            request.Order,
            request.OrderBy);
        var modelListAllCount = await _modelRepository.CountAsync(countSpecification);

        return new PagedResponse<Model>(
            request.Page,
            request.PageSize,
            modelListAllCount,
            modelList
        );
    }
}
