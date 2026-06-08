using Inventory.Domain.Common;
using Inventory.Domain.Specifications.Brands;

namespace Inventory.Domain.AggregatesModel.BrandAggregate;

public interface IBrandRepository : IRepository<Brand>
{
    public Task<IReadOnlyList<Brand>> ListAsync(BrandListPaginatedSpecification spec);
    public Task<int> CountAsync(BrandListCountSpecification spec);
}
