using System;

namespace Application.Models
{
    public class SignInResponce
    {
        public string Access_Token { get; set; }

        public string Refresh_Token { get; set; }

        public DateTime ExpiresIn { get; set; }
    }
}
