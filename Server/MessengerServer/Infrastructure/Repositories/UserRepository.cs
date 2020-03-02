using Domain.Entities;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepository:Repository<User>,IUserRepository
    {
        public UserRepository(MessengerContext db):base(db)
        {
            
        }

        public async Task<User> GetWithPhotoAsync(string userName)
        {
            return await this.db.Users.Where(u => u.Email == userName)
                .FirstOrDefaultAsync();
        }

        public async Task<List<User>> SearchUsersAsync(string filter)
        {
          return await this.db.Users
                .Where(u =>u.NickName.Contains(filter) || u.Email.Contains(filter))
                .Take(5)
                .ToListAsync();
        }

        public async Task<User> GetUserWithBlackList(string userName)
        {
            return await this.db.Users.Where(user => user.Email == userName)
                    .Include(user => user.BlockedUsers)
                    .FirstOrDefaultAsync();
        }
    }
}
