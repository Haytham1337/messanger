using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public class NameIdentifierMiddleware
    {
        private readonly RequestDelegate _next;

        public NameIdentifierMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public  async Task Invoke(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                context.Items["id"] =
                     int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            }

            await _next(context);       
        }
    }
}
