using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using Inventory.Application.Caching.Keys;
using Inventory.Application.Features.Brands.Dtos;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using Caching.Services;
using Inventory.Domain.Specifications.Brands;
using Inventory.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories;

public class CachedBrandRepository : 
    CachedRepository<Brand>, IBrandRepository
{
    public readonly IMapper _mapper;

    public CachedBrandRepository(
        InventoryDbContext dbContext,
        ICacheService cache,
        IMapper mapper) : base(dbContext, cache)
    {
        _mapper = mapper;
    }

    public override async Task<Guid> AddAsync(Brand brand)
    {
        await _dbSet.AddAsync(brand);
        await Invalidate(brand.Id);
        await _cache.IncrementVersionAsync(BrandCacheKeys.ListKey);
        await _cache.IncrementVersionAsync(BrandCacheKeys.ListCountKey);

        return brand.Id;
    }

    public override async Task AddRangeAsync(IEnumerable<Brand> brands)
    {
        await _dbSet.AddRangeAsync(brands);
        foreach (var brand in brands)
        {
            await Invalidate(brand.Id);
        }
        await _cache.IncrementVersionAsync(BrandCacheKeys.ListKey);
        await _cache.IncrementVersionAsync(BrandCacheKeys.ListCountKey);
    }

    public override async void Update(Brand brand)
    {
        _dbSet.Update(brand);
        await Invalidate(brand.Id);
        await _cache.IncrementVersionAsync(BrandCacheKeys.ListKey);
        await _cache.IncrementVersionAsync(BrandCacheKeys.ListCountKey);
    }

    public override async void Delete(Brand brand)
    {
        _dbSet.Remove(brand);
        await Invalidate(brand.Id);
        await _cache.IncrementVersionAsync(BrandCacheKeys.ListKey);
        await _cache.IncrementVersionAsync(BrandCacheKeys.ListCountKey);
    }

    public async Task<int> CountAsync(BrandListCountSpecification spec)
    {
        var version = await _cache.GetVersionAsync(BrandCacheKeys.ListCountKey);
        var cacheKey = spec.GetRedisCacheKey(version);
        var cached = await _cache.GetAsync<int?>(cacheKey);
        if (cached.HasValue)
        {
            return cached.Value;
        }
        var result = await _context.Brands
            .AsNoTracking()
            .WithSpecification(spec)
            .CountAsync();
        await _cache.SetAsync(cacheKey, result, TimeSpan.FromHours(6));
        return result;
    }

    public async Task<IReadOnlyList<Brand>> ListAsync(BrandListPaginatedSpecification spec)
    {
        var version = await _cache.GetVersionAsync(BrandCacheKeys.ListKey);
        var cacheKey = spec.GetRedisCacheKey(version);
        var cached = await _cache.GetAsync<IReadOnlyList<BrandCacheDto>>(cacheKey);
        if (cached?.Count > 0)
        {
            return _mapper.Map<IReadOnlyList<BrandCacheDto>, IReadOnlyList<Brand>>(cached);
        }
        var result = await _context.Brands
            .AsNoTracking()
            .WithSpecification(spec)
            .ToListAsync();
        var cachedModels = _mapper.Map<IReadOnlyList<BrandCacheDto>>(result);
        await _cache.SetAsync(cacheKey, cachedModels, TimeSpan.FromHours(6));
        return result;
    }
}