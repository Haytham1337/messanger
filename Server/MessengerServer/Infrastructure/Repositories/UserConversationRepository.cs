using Domain.Entities;
using Domain.IRepositories;

namespace Infrastructure.Repositories
{
    public class UserConversationRepository:Repository<UserConversation>,IUserConversationRepository
    {
        public UserConversationRepository(MessengerContext context):base(context)
        {

        }
    }
}
