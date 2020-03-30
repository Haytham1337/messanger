using Domain.Entities;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        public MessageRepository(MessengerContext db):base(db)
        {

        }

        public async Task<IEnumerable<Message>> GetMessagesByChat(int chatId,int portionCount)
        { 
            return await this.db.Messages
                 .Where(mes => mes.ChatId == chatId)
                 .Include(m => m.User)
                 .OrderByDescending(m => m.TimeCreated)
                 .Skip((portionCount-1)*10)
                 .Take(10)
                 .ToListAsync();
        }
    }
}
