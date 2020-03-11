using System.ComponentModel.DataAnnotations;

namespace Application.Models.ChatDto.Requests
{
    public class GetChatMessagesRequest
    {
        [Required]
        public int Id { get; set; }

        public int portion { get; set; } = 1;
    }
}
