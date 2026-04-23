using Identity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Services;

public interface IIdentityManagerService
{
    public Task<IdentityUser?> GetUserByIdAsync(string Id);
    public Task<bool> DeleteUserByIdAsync(string Id);
}
