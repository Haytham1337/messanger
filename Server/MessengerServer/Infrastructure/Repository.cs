using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected MessengerContext db;

        public Repository(MessengerContext context)
        {
            this.db = context;
        }

        public async Task CreateAsync(T item)
        {
            await db.Set<T>().AddAsync(item);
        }

        public async Task DeleteAsync(int id)
        {
            T entity = await db.Set<T>().FindAsync(id);
            if (entity != null)
                db.Set<T>().Remove(entity);
        }

        public async Task<T> GetAsync(int id)
        {
            return await db.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await db.Set<T>().ToListAsync();
        }

        public T Update(T item)
        {
            return db.Set<T>().Update(item)?.Entity;
        }
    }
}
