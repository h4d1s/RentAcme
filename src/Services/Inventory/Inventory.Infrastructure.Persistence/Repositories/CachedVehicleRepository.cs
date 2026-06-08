using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using Caching.Services;
using Inventory.Application.Caching.Keys;
using Inventory.Application.Features.Vehicles.Dtos;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Inventory.Domain.Specifications.Vehicles;
using Inventory.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories;

public class CachedVehicleRepository : CachedRepository<Vehicle>, IVehicleRepository
{
    public readonly IMapper _mapper;

    public CachedVehicleRepository(
        InventoryDbContext dbContext,
        ICacheService cacheService,
        IMapper mapper) : base(dbContext, cacheService)
    {
        _mapper = mapper;
    }

    public override async Task<Guid> AddAsync(Vehicle vehicle)
    {
        await _dbSet.AddAsync(vehicle);
        await Invalidate(vehicle.Id);
        await _cache.IncrementVersionAsync(VehicleCacheKeys.ListVersionKey);
        await _cache.IncrementVersionAsync(VehicleCacheKeys.ListCountVersionKey);
        await _cache.IncrementVersionAsync(VehicleCacheKeys.FilterVersionKey);
        await _cache.IncrementVersionAsync(VehicleCacheKeys.FilterCountVersionKey);

        return vehicle.Id;
    }

    public override async Task AddRangeAsync(IEnumerable<Vehicle> vehicles)
    {
        await _dbSet.AddRangeAsync(vehicles);
        foreach (var vehicle in vehicles)
        {
            await Invalidate(vehicle.Id);
        }
        await _cache.IncrementVersionAsync(VehicleCacheKeys.ListVersionKey);
        await _cache.IncrementVersionAsync(VehicleCacheKeys.ListCountVersionKey);
        await _cache.IncrementVersionAsync(VehicleCacheKeys.FilterVersionKey);
        await _cache.IncrementVersionAsync(VehicleCacheKeys.FilterCountVersionKey);
    }

    public override async void Update(Vehicle vehicle)
    {
        _dbSet.Update(vehicle);
        await Invalidate(vehicle.Id);
        await _cache.IncrementVersionAsync(VehicleCacheKeys.ListVersionKey);
        await _cache.IncrementVersionAsync(VehicleCacheKeys.ListCountVersionKey);
        await _cache.IncrementVersionAsync(VehicleCacheKeys.FilterVersionKey);
        await _cache.IncrementVersionAsync(VehicleCacheKeys.FilterCountVersionKey);
    }

    public override async void Delete(Vehicle vehicle)
    {
        _dbSet.Remove(vehicle);
        await Invalidate(vehicle.Id);
        await _cache.IncrementVersionAsync(VehicleCacheKeys.ListVersionKey);
        await _cache.IncrementVersionAsync(VehicleCacheKeys.ListCountVersionKey);
        await _cache.IncrementVersionAsync(VehicleCacheKeys.FilterVersionKey);
        await _cache.IncrementVersionAsync(VehicleCacheKeys.FilterCountVersionKey);
    }

    public async Task<int> CountAsync(VehicleListCountSpecification spec)
    {
        var version = await _cache.GetVersionAsync(VehicleCacheKeys.ListCountVersionKey);
        var cacheKey = spec.GetRedisCacheKey(version);
        var cached = await _cache.GetAsync<int?>(cacheKey);
        if (cached.HasValue)
        {
            return cached.Value;
        }
        var result = await _context.Vehicles
            .AsNoTracking()
            .WithSpecification(spec)
            .CountAsync();
        await _cache.SetAsync(cacheKey, result, TimeSpan.FromHours(6));
        return result;
    }

    public async Task<IReadOnlyList<Vehicle>> ListAsync(VehicleListPaginatedSpecification spec)
    {
        var version = await _cache.GetVersionAsync(VehicleCacheKeys.ListVersionKey);
        var cacheKey = spec.GetRedisCacheKey(version);
        var cached = await _cache.GetAsync<IReadOnlyList<VehicleCacheDto>>(cacheKey);
        if (cached?.Count > 0)
        {
            return _mapper.Map<IReadOnlyList<VehicleCacheDto>, IReadOnlyList<Vehicle>>(cached);
        }
        var result = await _context.Vehicles
            .AsNoTracking()
            .WithSpecification(spec)
            .ToListAsync();
        var cachedVehicles = _mapper.Map<IReadOnlyList<VehicleCacheDto>>(result);
        await _cache.SetAsync(cacheKey, cachedVehicles, TimeSpan.FromHours(6));
        return result;
    }

    public async Task<int> CountAsync(VehicleFilterCountSpecification spec)
    {
        var version = await _cache.GetVersionAsync(VehicleCacheKeys.FilterCountVersionKey);
        var cacheKey = spec.GetRedisCacheKey(version);
        var cached = await _cache.GetAsync<int?>(cacheKey);
        if (cached.HasValue)
        {
            return cached.Value;
        }
        var result = await _context.Vehicles
            .AsNoTracking()
            .WithSpecification(spec)
            .CountAsync();
        await _cache.SetAsync(cacheKey, result, TimeSpan.FromHours(6));
        return result;
    }

    public async Task<IReadOnlyList<Vehicle>> ListAsync(VehicleFilterPaginatedSpecification spec)
    {
        var version = await _cache.GetVersionAsync(VehicleCacheKeys.FilterVersionKey);
        var cacheKey = spec.GetRedisCacheKey(version);
        var cached = await _cache.GetAsync<IReadOnlyList<VehicleCacheDto>>(cacheKey);
        if (cached?.Count > 0)
        {
            return _mapper.Map<IReadOnlyList<VehicleCacheDto>, IReadOnlyList<Vehicle>>(cached);
        }
        var result = await _context.Vehicles
            .AsNoTracking()
            .WithSpecification(spec)
            .ToListAsync();
        var cachedVehicles = _mapper.Map<IReadOnlyList<VehicleCacheDto>>(result);
        await _cache.SetAsync(cacheKey, cachedVehicles, TimeSpan.FromHours(6));
        return result;
    }
}
