using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;

namespace Core.Extentsions
{
    public static class HttpContextExtensions
    {
        public static long GetUserId(this HttpContext httpContext)
        {
            var strUserId = httpContext?.User?.Claims?.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(x => x.Value).FirstOrDefault();
            if (string.IsNullOrEmpty(strUserId))
                throw new Exception("User not found.");
            return Convert.ToInt64(strUserId);
        }
    }
}
