using Identity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Services;

public interface IIdentityService
{
    public string? GetUserId();
    public string? GetUserEmail();
    public string? GetUserName();
    public List<string> GetUserRoles();
    public string? GetToken();
}
