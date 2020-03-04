using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure.AppSecurity
{
    public class JWToptions
    {
        public string  Issuer { get; set; }

        public string Audience { get; set; }

        public string Key { get; set; }

        public int LifeTime { get; set; }

        public  SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(this.Key));
        }
    }
}
