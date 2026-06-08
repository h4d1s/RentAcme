using User.Domain.Common;
using User.Domain.Specifications.ApplicationUsers;

namespace User.Domain.AggregatesModel.ApplicationUserAggregate;

public interface IApplicationUserRepository : IRepository<ApplicationUser>
{
    public Task<ApplicationUser?> GetByEmailAsync(string email);
    public Task<ApplicationUser?> GetByExternalIdAsync(string externalId);
    public Task<IReadOnlyList<ApplicationUser>> ListAsync(ApplicationUserListPaginatedSpecification spec);
    public Task<int> CountAsync(ApplicationUserListCountPaginatedSpecification spec);
}
