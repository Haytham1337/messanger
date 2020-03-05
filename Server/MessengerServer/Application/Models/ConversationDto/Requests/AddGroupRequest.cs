using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.Models.ConversationDto.Requests
{
    public class AddGroupRequest
    {
        public int UserId { get; set; }

        [Required]
        public string GroupName { get; set; }

        public bool IsChannel { get; set; }

        public List<int> UsersId { get; set; } = new List<int>();
    }
}
