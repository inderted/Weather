using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Weather.Services.Authentication
{
    public interface IBasicAuthMiddleware
    {
        Task<bool> Invoke(HttpContext context);

        bool IsAuthorized(string username, string password, HttpContext context);
    }
}
