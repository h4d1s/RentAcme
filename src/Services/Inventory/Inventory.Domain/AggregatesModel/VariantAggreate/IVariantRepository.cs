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
}
