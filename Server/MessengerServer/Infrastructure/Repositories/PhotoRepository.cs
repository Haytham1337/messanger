using Domain.Entities;
using Domain.IRepositories;

namespace Infrastructure.Repositories
{
    public class PhotoRepository:Repository<ConversationInfo>,IConversationInfoRepository
    {
        public PhotoRepository(MessengerContext db):base(db)
        {

        }
    }
}
