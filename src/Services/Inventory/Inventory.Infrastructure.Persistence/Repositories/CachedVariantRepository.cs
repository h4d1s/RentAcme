using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using Caching.Services;
using Inventory.Application.Caching.Keys;
using Inventory.Application.Features.Variants.Dtos;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Domain.Specifications.Models;
using Inventory.Domain.Specifications.Variants;
using Inventory.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories;

public class CachedVariantRepository
    : CachedRepository<Variant>, IVariantRepository
{
    public readonly IMapper _mapper;

    public CachedVariantRepository(
        InventoryDbContext dbContext,
        ICacheService cacheService,
        IMapper mapper) : base(dbContext, cacheService)
    {
        _mapper = mapper;
    }

    public override async Task<Guid> AddAsync(Variant variant)
    {
        await _dbSet.AddAsync(variant);
        await Invalidate(variant.Id);
        await _cache.IncrementVersionAsync(VariantCacheKeys.ListVersionKey);
        await _cache.IncrementVersionAsync(VariantCacheKeys.ListCountVersionKey);

        return variant.Id;
    }

    public override async Task AddRangeAsync(IEnumerable<Variant> variants)
    {
        await _dbSet.AddRangeAsync(variants);
        foreach (var variant in variants)
        {
            await Invalidate(variant.Id);
        }
        await _cache.IncrementVersionAsync(VariantCacheKeys.ListVersionKey);
        await _cache.IncrementVersionAsync(VariantCacheKeys.ListCountVersionKey);
    }

    public override async void Update(Variant variant)
    {
        _dbSet.Update(variant);
        await Invalidate(variant.Id);
        await _cache.IncrementVersionAsync(VariantCacheKeys.ListVersionKey);
        await _cache.IncrementVersionAsync(VariantCacheKeys.ListCountVersionKey);
    }

    public override async void Delete(Variant variant)
    {
        _dbSet.Remove(variant);
        await Invalidate(variant.Id);
        await _cache.IncrementVersionAsync(VariantCacheKeys.ListVersionKey);
        await _cache.IncrementVersionAsync(VariantCacheKeys.ListCountVersionKey);
    }

    public async Task<int> CountAsync(VariantListCountSpecification spec)
    {
        var version = await _cache.GetVersionAsync(VariantCacheKeys.ListCountVersionKey);
        var cacheKey = spec.GetRedisCacheKey(version);
        var cached = await _cache.GetAsync<int?>(cacheKey);
        if (cached.HasValue)
        {
            return cached.Value;
        }
        var result = await _context.Variants
            .AsNoTracking()
            .WithSpecification(spec)
            .CountAsync();
        await _cache.SetAsync(cacheKey, result, TimeSpan.FromHours(6));
        return result;
    }

    public async Task<IReadOnlyList<Variant>> ListAsync(VariantListPaginatedSpecification spec)
    {
        var version = await _cache.GetVersionAsync(VariantCacheKeys.ListVersionKey);
        var cacheKey = spec.GetRedisCacheKey(version);
        var cached = await _cache.GetAsync<IReadOnlyList<VariantCacheDto>>(cacheKey);
        if (cached?.Count > 0)
        {
            return _mapper.Map<IReadOnlyList<VariantCacheDto>, IReadOnlyList<Variant>>(cached);
        }
        var result = await _context.Variants
            .AsNoTracking()
            .WithSpecification(spec)
            .ToListAsync();
        var cachedVehicles = _mapper.Map<IReadOnlyList<VariantCacheDto>>(result);
        await _cache.SetAsync(cacheKey, cachedVehicles, TimeSpan.FromHours(6));
        return result;
    }
}
