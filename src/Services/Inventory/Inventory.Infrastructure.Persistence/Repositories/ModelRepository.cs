using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Infrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.Persistence.Repositories;

public class ModelRepository : Repository<Model>, IModelRepository
{
    public ModelRepository(InventoryContext context) : base(context)
    {
    }
}
