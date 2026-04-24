using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories;

public class ModelRepository : Repository<Model>, IModelRepository
{
    public ModelRepository(InventoryDbContext context) : base(context)
    {
    }

    public virtual async Task<IReadOnlyList<Model>> ListAsync(Specification<Model> spec)
    {
        return await _context.Models
            .AsNoTracking()
            .WithSpecification(spec)
            .ToListAsync();
    }

    public virtual async Task<int> CountAsync(Specification<Model> spec)
    {
        return await _context.Models
            .AsNoTracking()
            .WithSpecification(spec)
            .CountAsync();
    }
}
