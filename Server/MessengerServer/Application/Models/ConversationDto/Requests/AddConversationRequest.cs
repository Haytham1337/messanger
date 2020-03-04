using System.ComponentModel.DataAnnotations;

namespace Application.Models.ChatDto.Requests
{
    public class AddConversationRequest
    {
        [Required]
        public int id { get; set; }

        public int userId { get; set; }
    }
}
