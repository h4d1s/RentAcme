using User.Application.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;

namespace User.Infrastructure.Security;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var requiredPermisions = requirement.Permission;
        var requiredList = requiredPermisions.Split(',', StringSplitOptions.RemoveEmptyEntries);

        var permissions = context.User.FindAll("permissions").Select(x => x.Value).ToList();

        if (requiredList.Any(perm => permissions.Contains(perm.Trim())))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}