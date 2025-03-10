using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace Koru
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var permissionsClaims =
                context.User.Claims.Where(c => c.Type == "Permission");
            // If user does not have the scope claim, get out of here
            if (!permissionsClaims.Any())
                return Task.CompletedTask;

            if(permissionsClaims.Any(p => p.Value == requirement.PermissionName || p.Value == "SuperAdmin"))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
