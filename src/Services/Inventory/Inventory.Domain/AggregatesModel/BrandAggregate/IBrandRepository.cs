using Ardalis.Specification;
using Inventory.Domain.Common;

namespace Inventory.Domain.AggregatesModel.BrandAggregate;

public interface IBrandRepository : IRepository<Brand>
{
    public Task<IReadOnlyList<Brand>> ListAsync(Specification<Brand> spec);
    public Task<int> CountAsync(Specification<Brand> spec);
}
