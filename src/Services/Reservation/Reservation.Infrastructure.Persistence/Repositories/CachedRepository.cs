using Caching.Services;
using Catalog.Domain.Common;
using Reservation.Domain.Common;
using Reservation.Infrastructure.Persistence.Data;

namespace Reservation.Infrastructure.Persistence.Repositories;

public class CachedRepository<T> : IRepository<T>
    where T : Entity, IAggregateRoot
{
    protected readonly ReservationDbContext _context;
    public IUnitOfWork UnitOfWork => _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly ICacheService _cache;

    public CachedRepository(
        ReservationDbContext context,
        ICacheService cache)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _cache = cache;
    }

    protected async Task Invalidate(Guid id)
    {
        await _cache.RemoveAsync($"{typeof(T).Name}:{id}");
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        string key = $"{typeof(T).Name}:{id}";
        var cached = await _cache.GetAsync<T>(key);

        if (cached is not null)
        {
            return cached;
        }

        var entity = await _dbSet.FirstAsync(x => x.Id == id);
        if (entity is not null)
        {
            await _cache.SetAsync(
                key,
                entity,
                TimeSpan.FromMinutes(30));
        }

        return entity;
    }

    public virtual async Task<Guid> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await Invalidate(entity.Id);

        return entity.Id;
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        foreach (var entity in entities)
        {
            await Invalidate(entity.Id);
        }
    }

    public virtual async void Update(T entity)
    {
        _dbSet.Update(entity);
        await Invalidate(entity.Id);
    }

    public virtual async void Delete(T entity)
    {
        _dbSet.Remove(entity);
        await Invalidate(entity.Id);
    }

    public virtual Task<bool> AnyAsync()
    {
        return _dbSet.AnyAsync();
    }

    public virtual Task<int> CountAsync()
    {
        return _dbSet
            .AsNoTracking()
            .CountAsync();
    }
}