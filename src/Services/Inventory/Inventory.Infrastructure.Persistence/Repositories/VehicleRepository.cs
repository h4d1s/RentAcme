using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Inventory.Application.Models;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Inventory.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Inventory.Infrastructure.Persistence.Repositories;

public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(InventoryDbContext context) : base(context)
    {
    }

    public virtual async Task<IReadOnlyList<Vehicle>> ListAsync(Specification<Vehicle> spec)
    {
        return await _context.Vehicles
            .AsNoTracking()
            .WithSpecification(spec)
            .ToListAsync();
    }

    public virtual async Task<int> CountAsync(Specification<Vehicle> spec)
    {
        return await _context.Vehicles
            .AsNoTracking()
            .WithSpecification(spec)
            .CountAsync();
    }

    public async override Task<Vehicle?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Variant)
            .ThenInclude(x => x.Model)
            .ThenInclude(x => x.Brand)
            .FirstOrDefaultAsync(q => q.Id == id);
    }
}
