using Application;
using Application.IServices;
using Infrastructure.Cache;
using Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class ExtensionMethods
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
        {
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            return app;
        }

        public static IApplicationBuilder UseIdHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware(typeof(NameIdentifierMiddleware));

            return app;
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IMessageService, MessageService>();

            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IConversationService, ConversationService>();

            services.AddSingleton<ICache, MemoryCache>();

            services.AddScoped<IPhotoHelper, PhotoHelper>();
        }

        public static int GetUserId(this HttpContext context)
        {
            return (int)context.Items["id"];
        }
    }
}
