using Ardalis.Specification;
using Inventory.Domain.AggregatesModel.BookingAggregate;
using Inventory.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.AggregatesModel.VariantAggreate;

public interface IVariantRepository : IRepository<Variant>
{
    public Task<IReadOnlyList<Variant>> ListAsync(Specification<Variant> spec);
    public Task<int> CountAsync(Specification<Variant> spec);
}
