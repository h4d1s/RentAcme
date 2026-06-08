using Inventory.Domain.Common;
using Inventory.Domain.Specifications.Models;

namespace Inventory.Domain.AggregatesModel.ModelAggregate;

public interface IModelRepository : IRepository<Model>
{
    public Task<IReadOnlyList<Model>> ListAsync(ModelListPaginatedSpecification spec);
    public Task<int> CountAsync(ModelListCountSpecification spec);
}
