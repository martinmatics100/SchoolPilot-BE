

using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using System;
using System.Text;

namespace SchoolPilot.Host.Hangfire
{
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private const string DashboardUsername = "admin";
        private const string DashboardPassword = "password123";

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var authHeader = httpContext.Request.Headers["Authorization"];

            if (string.IsNullOrWhiteSpace(authHeader))
            {
                Challenge(httpContext);
                return false;
            }

            var authHeaderValue = authHeader.ToString();
            if (!authHeaderValue.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                Challenge(httpContext);
                return false;
            }

            var token = authHeaderValue.Substring("Basic ".Length).Trim();
            var credentialString = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var credentials = credentialString.Split(':', 2);

            if (credentials.Length != 2 ||
                credentials[0] != DashboardUsername ||
                credentials[1] != DashboardPassword)
            {
                Challenge(httpContext);
                return false;
            }
            // 👇 Allow access for now (you can add user role/IP checks here)
            return true;
        }

        private void Challenge(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 401;
            httpContext.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Hangfire Dashboard\"";
        }
    }
}
