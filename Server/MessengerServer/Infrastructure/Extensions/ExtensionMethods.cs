using Application;
using Application.IServices;
using Application.IServices.IHelpers;
using Infrastructure.Extensions.MiddleWares;
using Infrastructure.Services;
using Infrastructure.Services.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Web;

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

        public static IApplicationBuilder UseUserStatusMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware(typeof(UserStatusMiddleware));

            return app;
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IMessageService, MessageService>();

            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IConversationService, ConversationService>();

            services.AddScoped<IGroupService, GroupService>();

            services.AddSingleton<ICache, MemoryCache>();

            services.AddScoped<IPhotoHelper, PhotoHelper>();

            services.AddScoped<IJwtHelper, JwtHelper>();

            services.AddScoped<IEmailSenderHelper, EmailSenderHelper>();

            services.AddScoped<IProvidersAuthService, ProvidersAuthService>();
        }

        public static int GetUserId(this HttpContext context)
        {
            return (int)context.Items["id"];
        }
    }
}
