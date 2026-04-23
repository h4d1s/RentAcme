using Ardalis.Specification;
using Inventory.Domain.AggregatesModel.BookingAggregate;
using Inventory.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.AggregatesModel.ModelAggregate;

public interface IModelRepository : IRepository<Model>
{
    public Task<IReadOnlyList<Model>> ListAsync(Specification<Model> spec);
    public Task<int> CountAsync(Specification<Model> spec);
}
