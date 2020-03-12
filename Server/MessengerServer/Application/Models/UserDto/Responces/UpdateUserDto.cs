using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Models.UserDto
{
    public class UpdateUserDto
    {
        public int UserId { get; set; }
 
        [Range(1,100)]
        public int Age { get; set; }

        [Required]
        public string NickName { get; set; }

        [Phone]
        public string Phone { get; set; }
    }
}
