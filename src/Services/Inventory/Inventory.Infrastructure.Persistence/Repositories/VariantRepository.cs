using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories;

public class VariantRepository : Repository<Variant>, IVariantRepository
{
    public VariantRepository(InventoryDbContext context) : base(context)
    {
    }

    public virtual async Task<IReadOnlyList<Variant>> ListAsync(Specification<Variant> spec)
    {
        return await _context.Variants
            .AsNoTracking()
            .WithSpecification(spec)
            .ToListAsync();
    }

    public virtual async Task<int> CountAsync(Specification<Variant> spec)
    {
        return await _context.Variants
            .AsNoTracking()
            .WithSpecification(spec)
            .CountAsync();
    }
}
