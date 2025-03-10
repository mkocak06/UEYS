using Koru.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Koru
{
    public class UserValidationEvent : JwtBearerEvents
    {
        public override async Task TokenValidated(TokenValidatedContext context)
        {
            string adminIdStr = context.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(adminIdStr))
            {
                if (long.TryParse(adminIdStr,out long adminId))
                {
                    //Get EF context
                    var koruRepository = context.HttpContext.RequestServices.GetRequiredService<IKoruRepository>();
                    //var rtoPCalcer = new CalcAllowedPermissions(roleDbContext);
                    string[] userPermissions = await koruRepository.GetUserPermissionsAsync(adminId);
                    var userPermissionList = userPermissions.Select(p => new Claim("Permission", p));

                    var claimsIdentity = (ClaimsIdentity)context.Principal.Identity;
                    claimsIdentity.AddClaims(userPermissionList);
                }
                
            }
        }
    }
}
