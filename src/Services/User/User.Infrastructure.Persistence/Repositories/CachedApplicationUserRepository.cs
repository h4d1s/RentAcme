using Microsoft.EntityFrameworkCore;
using User.Application.Caching.Keys;
using User.Domain.AggregatesModel.ApplicationUserAggregate;
using User.Domain.Specifications.ApplicationUsers;
using User.Infrastructure.Persistence.Data;
using User.Application.Features.Users.Dtos;
using Caching.Services;
using AutoMapper;
using Ardalis.Specification.EntityFrameworkCore;

namespace User.Infrastructure.Persistence.Repositories;

public class CachedApplicationUserRepository : CachedRepository<ApplicationUser>, IApplicationUserRepository
{
    public readonly ICacheService _cacheService;
    public readonly IMapper _mapper;

    public CachedApplicationUserRepository(
        ApplicationUserDbContext context,
        ICacheService cache,
        IMapper mapper) : base(context, cache)
    {
        _cacheService = cache;
        _mapper = mapper;
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Email == email);
    }

    public async Task<ApplicationUser?> GetByExternalIdAsync(string externalId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.ExternalId == externalId);
    }

    public async Task<IReadOnlyList<ApplicationUser>> ListAsync(ApplicationUserListPaginatedSpecification spec)
    {
        var version = await _cache.GetVersionAsync(ApplicationUserCacheKeys.ListVersionKey);
        var cacheKey = spec.GetRedisCacheKey(version);
        var cached = await _cache.GetAsync<IReadOnlyList<ApplicationUserCacheDto>>(cacheKey);
        if (cached?.Count > 0)
        {
            return _mapper.Map<IReadOnlyList<ApplicationUserCacheDto>, IReadOnlyList<ApplicationUser>>(cached);
        }
        var result = await _context.ApplicationUsers
            .AsNoTracking()
            .WithSpecification(spec)
            .ToListAsync();
        var cachedModels = _mapper.Map<IReadOnlyList<ApplicationUserCacheDto>>(result);
        await _cache.SetAsync(cacheKey, cachedModels, TimeSpan.FromHours(6));
        return result;
    }

    public async Task<int> CountAsync(ApplicationUserListCountPaginatedSpecification spec)
    {
        var version = await _cache.GetVersionAsync(ApplicationUserCacheKeys.ListCountVersionKey);
        var cacheKey = spec.GetRedisCacheKey(version);
        var cached = await _cache.GetAsync<int?>(cacheKey);
        if (cached.HasValue)
        {
            return cached.Value;
        }
        var result = await _context.ApplicationUsers
            .AsNoTracking()
            .WithSpecification(spec)
            .CountAsync();
        await _cache.SetAsync(cacheKey, result, TimeSpan.FromHours(6));
        return result;
    }
}
