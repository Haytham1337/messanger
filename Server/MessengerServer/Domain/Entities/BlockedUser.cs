namespace Domain.Entities
{
    public class BlockedUser
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int UserToBlockId { get; set; }
        public User UserToBlock { get; set; }
    }
}
