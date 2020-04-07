using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.IRepositories
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<IEnumerable<Message>> GetMessagesByChat(int chatId, int portionCount);

        Task<Message> GetMessageByIdWithConversationInfo(int id);
    }
}
