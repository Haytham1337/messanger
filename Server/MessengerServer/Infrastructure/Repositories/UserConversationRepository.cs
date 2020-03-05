using Domain.Entities;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserConversationRepository:Repository<UserConversation>,IUserConversationRepository
    {
        public UserConversationRepository(MessengerContext context):base(context)
        {

        }

        public async Task<List<UserConversation>> GetUsersByConversationAsync(int id)
        {
            return await this.db.UserConversations
                .Where(uconv => uconv.ConversationId == id)
                .Include(uconv => uconv.User)
                .ToListAsync();
        }
    }
}
