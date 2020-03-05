using Domain.Entities;

namespace Application.Models.ConversationDto.Responces
{
    public class SearchConversationResponce
    {
        public int id { get; set; }

        public string Name { get; set; }

        public string Photo { get; set; }

        public ConversationType Type { get; set; }
    }
}
