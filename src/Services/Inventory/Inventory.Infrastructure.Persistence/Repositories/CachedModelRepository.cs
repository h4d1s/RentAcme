using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using Caching.Services;
using Inventory.Application.Caching.Keys;
using Inventory.Application.Features.Models.Dtos;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.Specifications.Models;
using Inventory.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories;

public class CachedModelRepository : CachedRepository<Model>, IModelRepository
{
    public readonly IMapper _mapper;

    public CachedModelRepository(
        InventoryDbContext dbContext,
        ICacheService cacheService,
        IMapper mapper) : base(dbContext, cacheService)
    {
        _mapper = mapper;
    }

    public override async Task<Guid> AddAsync(Model model)
    {
        await _dbSet.AddAsync(model);
        await Invalidate(model.Id);
        await _cache.IncrementVersionAsync(ModelCacheKeys.ListVersionKey);
        await _cache.IncrementVersionAsync(ModelCacheKeys.ListCountVersionKey);

        return model.Id;
    }

    public override async Task AddRangeAsync(IEnumerable<Model> models)
    {
        await _dbSet.AddRangeAsync(models);
        foreach (var model in models)
        {
            await Invalidate(model.Id);
        }
        await _cache.IncrementVersionAsync(ModelCacheKeys.ListVersionKey);
        await _cache.IncrementVersionAsync(ModelCacheKeys.ListCountVersionKey);
    }

    public override async void Update(Model model)
    {
        _dbSet.Update(model);
        await Invalidate(model.Id);
        await _cache.IncrementVersionAsync(ModelCacheKeys.ListVersionKey);
        await _cache.IncrementVersionAsync(ModelCacheKeys.ListCountVersionKey);
    }

    public override async void Delete(Model model)
    {
        _dbSet.Remove(model);
        await Invalidate(model.Id);
        await _cache.IncrementVersionAsync(ModelCacheKeys.ListVersionKey);
        await _cache.IncrementVersionAsync(ModelCacheKeys.ListCountVersionKey);
    }

    public async Task<int> CountAsync(ModelListCountSpecification spec)
    {
        var version = await _cache.GetVersionAsync(ModelCacheKeys.ListCountVersionKey);
        var cacheKey = spec.GetRedisCacheKey(version);
        var cached = await _cache.GetAsync<int?>(cacheKey);
        if (cached.HasValue)
        {
            return cached.Value;
        }
        var result = await _context.Models
            .AsNoTracking()
            .WithSpecification(spec)
            .CountAsync();
        await _cache.SetAsync(cacheKey, result, TimeSpan.FromHours(6));
        return result;
    }

    public async Task<IReadOnlyList<Model>> ListAsync(ModelListPaginatedSpecification spec)
    {
        var version = await _cache.GetVersionAsync(ModelCacheKeys.ListVersionKey);
        var cacheKey = spec.GetRedisCacheKey(version);
        var cached = await _cache.GetAsync<IReadOnlyList<ModelCacheDto>>(cacheKey);
        if (cached?.Count > 0)
        {
            return _mapper.Map<IReadOnlyList<ModelCacheDto>, IReadOnlyList<Model>>(cached);
        }
        var result = await _context.Models
            .AsNoTracking()
            .WithSpecification(spec)
            .ToListAsync();
        var cachedModels = _mapper.Map<IReadOnlyList<ModelCacheDto>>(result);
        await _cache.SetAsync(cacheKey, cachedModels, TimeSpan.FromHours(6));
        return result;
    }
}
