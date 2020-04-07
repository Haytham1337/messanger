using System.ComponentModel.DataAnnotations;

namespace Application.Models.MessageDto
{
    public class AddMessageDto
    {
        public string timeCreated { get; set; }
        public string Content { get; set; }

        public string photo { get; set; }

        public int userId { get; set; }

        [Required]
        public int chatId { get; set; }
    }
}
