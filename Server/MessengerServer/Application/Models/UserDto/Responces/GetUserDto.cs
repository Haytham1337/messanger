using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models.UserDto
{
    public class GetUserDto
    {
        public int Id { get; set; }

        public string PhotoName { get; set; }

        public string NickName { get; set; }

        public int Age { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public bool isOnline { get; set; }
    }
}
