using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using Caching.Services;
using Reservation.Application.Caching.Keys;
using Reservation.Application.Features.Bookings.Dtos;
using Reservation.Domain.AggregatesModel.BookingAggregate;
using Reservation.Domain.Specifications.Bookings;
using Reservation.Infrastructure.Persistence.Data;

namespace Reservation.Infrastructure.Persistence.Repositories;

public class CachedBookingRepository : CachedRepository<Booking>, IBookingRepository
{
    public readonly IMapper _mapper;

    public CachedBookingRepository(
        ReservationDbContext context,
        ICacheService cache,
        IMapper mapper)
        : base(context, cache)
    {
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<Booking>> ListAsync(Specification<Booking> spec)
    {
        return await _context.Bookings
            .AsNoTracking()
            .WithSpecification(spec)
            .ToListAsync();
    }

    public async Task<int> CountAsync(Specification<Booking> spec)
    {
        return await _context.Bookings
            .AsNoTracking()
            .WithSpecification(spec)
            .CountAsync();
    }

    public async Task<IReadOnlyList<Booking>> ListAsync(BookingListPaginatedSpecification spec)
    {
        var version = await _cache.GetVersionAsync(BookingCacheKeys.BookingsListKey);
        var cacheKey = spec.GetRedisCacheKey(version);
        var cached = await _cache.GetAsync<IReadOnlyList<BookingCacheDto>>(cacheKey);
        if (cached?.Count > 0)
        {
            return _mapper.Map<IReadOnlyList<BookingCacheDto>, IReadOnlyList<Booking>>(cached);
        }
        var result = await _context.Bookings
            .AsNoTracking()
            .WithSpecification(spec)
            .ToListAsync();
        var cachedModels = _mapper.Map<IReadOnlyList<BookingCacheDto>>(result);
        await _cache.SetAsync(cacheKey, cachedModels, TimeSpan.FromHours(6));
        return result;
    }

    public async Task<int> CountAsync(BookingListCountPaginatedSpecification spec)
    {
        var version = await _cache.GetVersionAsync(BookingCacheKeys.BookingsListCountKey);
        var cacheKey = spec.GetRedisCacheKey(version);
        var cached = await _cache.GetAsync<int?>(cacheKey);
        if (cached.HasValue)
        {
            return cached.Value;
        }
        var result = await _context.Bookings
            .AsNoTracking()
            .WithSpecification(spec)
            .CountAsync();
        await _cache.SetAsync(cacheKey, result, TimeSpan.FromHours(6));
        return result;
    }
}
