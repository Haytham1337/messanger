using System.ComponentModel.DataAnnotations;

namespace Application.Models.UserDto
{
    public class SearchRequest
    {
        [Required]
        public string Filter { get; set; }

        public int UserId { get; set; }
    }
}
