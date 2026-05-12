using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Identity.Services;

public class IdentityService : IIdentityService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IdentityService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public string? GetUserEmail()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
    }

    public string? GetUserName()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
    }

    public List<string> GetUserRoles()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
        {
            return [];
        }

        return user
            .FindAll(ClaimTypes.Role)
            .Select(r => r.Value)
            .ToList();
    }

    public List<string> GetUserPermissions()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
        {
            return [];
        }

        var permissionsClaim = user.FindAll("permissions");

        if (permissionsClaim is null)
        {
            return [];
        }

        return permissionsClaim
            .Select(c => c.Value)
            .ToList();
    }
}
