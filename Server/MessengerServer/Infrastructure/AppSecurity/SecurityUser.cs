using Microsoft.AspNetCore.Identity;

namespace Infrastructure.AppSecurity
{
    public class SecurityUser:IdentityUser<int>
    {
        public string RefreshToken { get; set; }
    }
}
