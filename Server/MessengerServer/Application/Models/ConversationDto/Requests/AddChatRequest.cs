using System.ComponentModel.DataAnnotations;

namespace Application.Models.ChatDto.Requests
{
    public class AddChatRequest
    {
        [Required]
        public int SecondUserId { get; set; }

        public int userId { get; set; }
    }
}
