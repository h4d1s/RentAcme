using Inventory.Domain.Common;
using Inventory.Domain.Specifications.Models;
using Inventory.Domain.Specifications.Variants;

namespace Inventory.Domain.AggregatesModel.VariantAggreate;

public interface IVariantRepository : IRepository<Variant>
{
    public Task<IReadOnlyList<Variant>> ListAsync(VariantListPaginatedSpecification spec);
    public Task<int> CountAsync(VariantListCountSpecification spec);
}
