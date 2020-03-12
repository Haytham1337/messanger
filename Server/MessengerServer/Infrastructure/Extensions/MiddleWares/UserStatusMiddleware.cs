using Application;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Infrastructure.Extensions.MiddleWares
{
    public class UserStatusMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICache _cache;

        public UserStatusMiddleware(RequestDelegate next, ICache cache)
        {
            _next = next;

            _cache = cache;
        }

        public async Task Invoke(HttpContext context)
        {

            if (context.User.Identity.IsAuthenticated)
            {
                var id= int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                _cache.Set($"{id}", true ,TimeSpan.FromSeconds(60));
            }

            await _next(context);
        }
    }
}
