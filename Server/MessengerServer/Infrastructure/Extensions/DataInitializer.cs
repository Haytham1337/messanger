using Domain.Entities;
using Infrastructure.AppSecurity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class DataInitializer
    {
        public static async Task SeedData(UserManager<SecurityUser> userManager, 
            RoleManager<IdentityRole<int>> roleManager, SecurityContext context, MessengerContext mescontext, IConfiguration config)
        {
            await SeedRoles(roleManager);
            await SeedUsers(userManager, context, mescontext, config);
        }
        public static async Task SeedUsers(UserManager<SecurityUser> userManager, 
            SecurityContext context, MessengerContext mescontext, IConfiguration _config)
        {
            string username = "admin@gmail.com";
            string password = "mainadmin";
            if (await userManager.FindByNameAsync(username) == null)
            {
                SecurityUser secadmin = new SecurityUser() { UserName = username, Email = username };
                IdentityResult result = await userManager.CreateAsync(secadmin, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(secadmin, "Admin"); 
                    await userManager.AddToRoleAsync(secadmin, "Chatter");
                }

                User admin = new User() 
                { 
                    NickName = "admin_captain", 
                    Sex = Sex.Male, 
                    Email = username,
                    Id=secadmin.Id,
                    Photo = _config.GetValue<string>("defaultmale")
                };
                
                mescontext.Users.Add(admin);

                mescontext.SaveChanges();
             }
        }
        public static async Task SeedRoles(RoleManager<IdentityRole<int>> roleManager)
        {
            string[] roleNames = { "Chatter", "Admin" };
            IdentityResult roleResult;
            foreach (var role in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(role);
                if (roleExist == false)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole<int>(role));
                }
            }
        }
    }
}
