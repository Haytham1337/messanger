using Domain.Entities;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ConversationRepository : Repository<Conversation>, IConversationRepository
    {
        public ConversationRepository(MessengerContext db) : base(db)
        {
        }

        public async Task<bool> ChatExistAsync(int firstUserId, int secondUserId)
        {
            return await this.db.Conversations
                 .Where(conv => conv.Type == ConversationType.Chat)
                 .Include(conv => conv.UserConversations)
                 .Where(conv => conv.UserConversations.Any(uconv => uconv.UserId == firstUserId)
                    && conv.UserConversations.Any(uconv => uconv.UserId == secondUserId))
                 .CountAsync() == 0;
        }

        public async Task<List<Conversation>> GetUserChatsAsync(int userid)
        {
            return await this.db.Conversations
                .Include(conv => conv.ConversationInfo)
                .Include(conv => conv.UserConversations)
                .Where(conv => conv.UserConversations.Any(uconv => uconv.UserId == userid))
                .Include(c => c.LastMessage)
                .OrderByDescending(c => c.LastMessage.TimeCreated)
                .ToListAsync();
        }

        public async Task<Conversation> GetChatContentAsync(int id)
        {
            return await this.db.Conversations
                  .Where(c => c.Id == id)
                  .Include(conv => conv.ConversationInfo)
                  .FirstOrDefaultAsync();
        }

        public async Task<Conversation> GetWithUsersConversationsAsync(int id)
        {
            return await db.Conversations
                         .Include(conv => conv.UserConversations)
                         .Include(conv => conv.ConversationInfo)
                         .FirstOrDefaultAsync(conv => conv.Id == id);
        }

        public async Task<List<Conversation>> SearchConversationsAsync(string filter, int userId)
        {
            return await this.db.Conversations
                .Include(conv => conv.UserConversations)
                .Where(conv => (conv.Type == ConversationType.Channel || (conv.Type == ConversationType.Group &&
                   conv.UserConversations.Any(uconv => uconv.UserId == userId))))
                .Include(conv => conv.ConversationInfo)
                .Where(conv => conv.ConversationInfo.GroupName.Contains(filter))
                .Take(5)
                .ToListAsync();
        }

        public async Task<bool> isUserConversationMember(int conversationId, int userId)
        {
            var conversation = await this.db.Conversations
                 .Where(conv => conv.Id == conversationId)
                 .Include(conv => conv.UserConversations)
                 .Where(conv => conv.UserConversations.Any(uconv => uconv.UserId == userId))
                 .FirstOrDefaultAsync();

            return conversation != null;
        }
    }
}