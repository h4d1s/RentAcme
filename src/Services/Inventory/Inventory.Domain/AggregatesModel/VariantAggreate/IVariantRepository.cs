using Ardalis.Specification;
using Inventory.Domain.Common;

namespace Inventory.Domain.AggregatesModel.VariantAggreate;

public interface IVariantRepository : IRepository<Variant>
{
    public Task<IReadOnlyList<Variant>> ListAsync(Specification<Variant> spec);
    public Task<int> CountAsync(Specification<Variant> spec);
}
