using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Exercises.Data;
using Microsoft.AspNetCore.Http;

namespace Exercises.Core.Helpers
{
    public static class GeneralExtensions
    {
        public static Guid GetUserId(this HttpContext httpContext)
        {
            if (httpContext.User == null || httpContext.User.Identity.IsAuthenticated == false)
            {
                return Guid.Empty;
            }

            var id = httpContext.User.Claims.Single(x => x.Type == "id").Value;
            return Guid.Parse(id);
        }

        public static IEnumerable<string> GetUserRoles(this HttpContext httpContext)
        {
            if (httpContext.User == null || httpContext.User.Identity.IsAuthenticated == false)
            {
                return new List<string>
                {
                    Constants.ROLE_ANONYMOUS
                };
            }

            return httpContext.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);
        }
    }
}