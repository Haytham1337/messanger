using System.ComponentModel.DataAnnotations;

namespace Application.Models.ConversationDto.Requests
{
    public class AddConversationMemberRequest
    {
        public int UserId { get; set; }

        [Required]
        public int UserToAdd { get; set; }

        [Required]
        public int ConversationId { get; set; }
    }
}
