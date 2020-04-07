using Domain.Entities;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class BlockedUserRepository : Repository<BlockedUser>, IBlockedUserRepository
    {
        public BlockedUserRepository(MessengerContext db) : base(db)
        {

        }

        public async Task<BlockedUser> IsBlockedUserAsync(int userId, int blockedUserId)
        {
            return await this.db.BlockedUsers
                .Where(blockedUser => blockedUser.UserId == userId && blockedUser.UserToBlockId == blockedUserId)
                .FirstOrDefaultAsync();
        }
    }
}
