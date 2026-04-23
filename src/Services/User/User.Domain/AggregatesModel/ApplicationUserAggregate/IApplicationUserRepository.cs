using User.Domain.Common;

namespace User.Domain.AggregatesModel.ApplicationUserAggregate;

public interface IApplicationUserRepository : IRepository<ApplicationUser>
{
    public Task<ApplicationUser?> GetByEmailAsync(string email);
    public Task<ApplicationUser?> GetByExternalIdAsync(string externalId);
}
