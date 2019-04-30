using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Remotion.Linq.Utilities;

namespace Weather.Services.Authentication
{
    public class BasicAuthMiddleware : IBasicAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _realm;

        public BasicAuthMiddleware(RequestDelegate next, string realm)
        {
            _next = next;
            _realm = realm;
        }

        public async Task<bool> Invoke(HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                // Get the encoded username and password
                var encodedUsernamePassword = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();
                // Decode from Base64 to string
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                // Split username and password
                var username = decodedUsernamePassword.Split(':', 2)[0];
                var password = decodedUsernamePassword.Split(':', 2)[1];
                // Check if login is correct
                if (IsAuthorized(username, password, context))
                {
                    await _next.Invoke(context);
                    return true;
                }
            }
            // Return authentication type (causes browser to show login dialog)
            context.Response.Headers["WWW-Authenticate"] = "Basic";
            // Add realm if it is not null
            if (!string.IsNullOrWhiteSpace(_realm))
            {
                context.Response.Headers["WWW-Authenticate"] += $" realm=\"{_realm}\"";
            }
            // Return unauthorized
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return false;
        }

        // Make your own implementation of this
        public bool IsAuthorized(string username, string password, HttpContext context)
        {
            var config = context.RequestServices.GetRequiredService<IConfiguration>();

            var basicAuthUserName = config["BasicAuth:UserName"];
            var basicAuthPassword = config["BasicAuth:Password"];
            // Check that username and password are correct
            return username.Equals(basicAuthUserName, StringComparison.InvariantCultureIgnoreCase)
                   && password.Equals(basicAuthPassword);
        }
    }
}
