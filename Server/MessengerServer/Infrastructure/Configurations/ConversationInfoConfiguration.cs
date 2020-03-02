using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ConversationInfoConfiguration : IEntityTypeConfiguration<ConversationInfo>
    {
        public void Configure(EntityTypeBuilder<ConversationInfo> builder)
        {
            builder.ToTable("ConversationInfo");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.PhotoName)
                   .IsRequired();

            builder.HasOne(c => c.Admin);

            builder.HasOne(c => c.Conversation)
                .WithOne(c => c.ConversationInfo)
                .HasForeignKey<ConversationInfo>(c => c.ConversationId);
        }
    }
}
