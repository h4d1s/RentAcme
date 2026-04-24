using Identity.Models;

namespace Identity.Services;

public interface IIdentityManagerService
{
    public Task<IdentityUser?> GetUserByIdAsync(string Id);
    public Task<bool> DeleteUserByIdAsync(string Id);
}
