using Domain.Entities;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class MessengerContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Conversation> Conversations { get; set; }

        public DbSet<BlockedUser> BlockedUsers { get; set; }

        public DbSet<UserConversation> UserConversations { get; set; }

        public DbSet<ConversationInfo> ConversationsInfo { get; set; }

        public MessengerContext(DbContextOptions<MessengerContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());

            modelBuilder.ApplyConfiguration(new MessageConfiguration());

            modelBuilder.ApplyConfiguration(new ConversationConfiguration());

            modelBuilder.ApplyConfiguration(new BlockedUserConfiguration());

            modelBuilder.ApplyConfiguration(new ConversationInfoConfiguration());

            modelBuilder.ApplyConfiguration(new UserConversationConfiguration());
        }
    }
}
