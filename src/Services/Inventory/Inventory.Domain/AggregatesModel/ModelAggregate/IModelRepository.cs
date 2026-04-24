using Ardalis.Specification;
using Inventory.Domain.Common;

namespace Inventory.Domain.AggregatesModel.ModelAggregate;

public interface IModelRepository : IRepository<Model>
{
    public Task<IReadOnlyList<Model>> ListAsync(Specification<Model> spec);
    public Task<int> CountAsync(Specification<Model> spec);
}
