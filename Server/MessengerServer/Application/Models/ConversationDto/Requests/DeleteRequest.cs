using System.ComponentModel.DataAnnotations;

namespace Application.Models.ConversationDto.Requests
{
    public class DeleteRequest
    {
        public int UserId { get; set; }

        [Required]
        public int ConversationId { get; set; }
    }
}
