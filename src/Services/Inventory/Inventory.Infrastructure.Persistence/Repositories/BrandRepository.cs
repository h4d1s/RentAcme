using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Inventory.Application.Models;
using Inventory.Domain.AggregatesModel.BookingAggregate;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Inventory.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Inventory.Infrastructure.Persistence.Repositories;

public class BrandRepository : Repository<Brand>, IBrandRepository
{
    public BrandRepository(InventoryDbContext context) : base(context)
    {
    }

    public virtual async Task<IReadOnlyList<Brand>> ListAsync(Specification<Brand> spec)
    {
        return await _context.Brands
            .AsNoTracking()
            .WithSpecification(spec)
            .ToListAsync();
    }

    public virtual async Task<int> CountAsync(Specification<Brand> spec)
    {
        return await _context.Brands
            .AsNoTracking()
            .WithSpecification(spec)
            .CountAsync();
    }
}
