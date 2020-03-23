using System.ComponentModel.DataAnnotations;

namespace Application.Models.ChatDto.Requests
{
    public class GetChatMessagesRequest
    {
        [Required]
        public int Id { get; set; }

        [Range(1,int.MaxValue)]
        public int portion { get; set; }=1;
    }
}
