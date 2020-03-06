using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
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
            catch(Exception ex)
            {
                context.Response.Clear();

                context.Response.ContentType = "application/json";

                if (ex is BaseException)
                    context.Response.StatusCode = ((BaseException)ex).StatusCode;
                else
                    context.Response.StatusCode = 400;

                await context.Response.WriteAsync(ex.Message);
            }
        }
    }
}
