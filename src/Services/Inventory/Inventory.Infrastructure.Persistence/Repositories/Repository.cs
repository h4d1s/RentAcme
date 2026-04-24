using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Inventory.Domain.Common;
using Inventory.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : Entity, IAggregateRoot
{
    protected readonly InventoryDbContext _context;
    public IUnitOfWork UnitOfWork => _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(InventoryDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public virtual async Task<IReadOnlyList<T>> ListAllAsync()
    {
        return await _dbSet
            .ToListAsync();
    }

    public virtual async Task<Guid> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity.Id;
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public virtual void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual async Task<bool> AnyAsync()
    {
        return await _dbSet
            .AnyAsync();
    }

    public virtual async Task<int> CountAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .CountAsync();
    }
}
