using Application.Models.UserDto;
using Domain.Entities;
using System.Collections.Generic;

namespace Application.Models.MessageDto
{
    public class AllMessagesDto
    {
        public List<GetUserDto> Users { get; set; }

        public List<GetMessageDto> Messages { get; set; }

        public ConversationType Type { get; set; }

        public int? AdminId  { get; set; }

        public string Name { get; set; }
    }
}
