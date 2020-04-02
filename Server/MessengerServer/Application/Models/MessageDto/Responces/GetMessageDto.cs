using System;

namespace Application.Models.MessageDto
{
    public class GetMessageDto
    {
        public int MessageId { get; set; }

        public string Photo { get; set; }

        public string messagePhoto { get; set; }

        public string Content { get; set; }

        public DateTime? TimeCreated { get; set; }

        public int UserId { get; set; }
    }
}
