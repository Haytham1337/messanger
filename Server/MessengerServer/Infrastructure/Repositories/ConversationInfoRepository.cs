using Domain.Entities;
using Domain.IRepositories;

namespace Infrastructure.Repositories
{
    public class ConversationInfoRepository:Repository<ConversationInfo>,IConversationInfoRepository
    {
        public ConversationInfoRepository(MessengerContext context):base(context)
        {
        }
    }
}
