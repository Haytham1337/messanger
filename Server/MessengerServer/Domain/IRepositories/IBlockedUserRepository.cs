using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IRepositories
{
    public interface IBlockedUserRepository : IRepository<BlockedUser>
    {
        Task<BlockedUser> IsBlockedUserAsync(int userId, int blockedUserId);
    }
}
