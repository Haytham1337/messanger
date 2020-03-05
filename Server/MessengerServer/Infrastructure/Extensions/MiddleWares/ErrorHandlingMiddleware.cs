using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                  await _next(context);
            }
            catch(BaseException ex)
            {
                context.Response.ContentType = "application/json";

                context.Response.StatusCode = ex.StatusCode;

                await context.Response.WriteAsync(ex.Message);
            }
        }
    }
}
