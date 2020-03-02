namespace Domain.Entities
{
    public class ConversationInfo
    {
        public int Id { get; set; }

        public int AdminId { get; set; }
        public User Admin { get; set; }

        public string PhotoName { get; set; }

        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }
    }
}
