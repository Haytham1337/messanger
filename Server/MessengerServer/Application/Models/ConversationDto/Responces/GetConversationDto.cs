using Domain.Entities;
using System;

namespace Application.Models.ChatDto.Responces
{
    public class GetConversationDto
    {
        public int Id { get; set; }

        public string Photo { get; set; }

        public string Content { get; set; }

        public int? SecondUserId { get; set; }

        public bool? IsBlocked { get; set; }

        public bool isOnline { get; set; }

        public DateTime? messageTime { get; set; }
    }
}
