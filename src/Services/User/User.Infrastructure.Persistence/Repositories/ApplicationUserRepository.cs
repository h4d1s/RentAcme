using Microsoft.EntityFrameworkCore;
using User.Domain.AggregatesModel.ApplicationUserAggregate;
using User.Infrastructure.Persistence.Data;

namespace User.Infrastructure.Persistence.Repositories;

public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
{
    public ApplicationUserRepository(ApplicationUserDbContext context) : base(context)
    {
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Email == email);
    }

    public async Task<ApplicationUser?> GetByExternalIdAsync(string externalId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.ExternalId == externalId);
    }
}
