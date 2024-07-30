using Inventory.Domain.Common;
using System.Linq.Expressions;

namespace Inventory.Domain.Common;

public interface IRepository<T> where T : Entity
{
    Task<int> CountAsync();
    Task<int> CountAsync(ISpecification<T> spec);
    Task<T?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<T>> ListAllAsync();
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
    Task<Guid> AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    void Update(T entity);
    void Delete(T entity);
    Task<bool> AnyAsync();
}
