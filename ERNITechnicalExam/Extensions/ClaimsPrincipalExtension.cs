using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static Guid? GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            Guid? userId = null;
            var userIdString = claimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var parsedGuid))
            {
                userId = parsedGuid;
            }

            return userId;
        }
        public static IEnumerable<string> GetUserType(this ClaimsPrincipal claimsPrincipal)
        {
            var roles = ((ClaimsIdentity)claimsPrincipal.Identity).Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);

            return roles;
        }
    }
}
