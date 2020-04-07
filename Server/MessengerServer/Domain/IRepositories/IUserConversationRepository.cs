using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.IRepositories
{
    public interface IUserConversationRepository : IRepository<UserConversation>
    {
        Task<List<UserConversation>> GetUsersByConversationAsync(int id);
    }
}
