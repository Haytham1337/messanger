using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Message
    {
        public int Id { get; private set; }

        public string Content { get; set; }

        public DateTime? TimeCreated { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int? ChatId { get; set; }
        public Conversation Chat { get; set; }
    }
}
