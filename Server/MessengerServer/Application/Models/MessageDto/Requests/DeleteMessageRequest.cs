using System.ComponentModel.DataAnnotations;

namespace Application.Models.MessageDto.Requests
{
    public class DeleteMessageRequest
    {
        public int UserId { get; set; }

        [Required]
        public int MessageId { get; set; }
    }
}
