using System.ComponentModel.DataAnnotations;

namespace Application.Models.ConversationDto.Requests
{
    public class LeaveGroupRequest
    {
        public int UserId { get; set; }

        [Required]
        public int UserToLeaveId { get; set; }

        [Required]
        public int ConversationId { get; set; }
    }
}
