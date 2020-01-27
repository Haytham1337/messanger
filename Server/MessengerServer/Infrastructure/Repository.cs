﻿using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private MessengerContext db;

        public Repository(MessengerContext context)
        {
            this.db = context;
        }

        public async Task Create(T item)
        {
            await db.Set<T>().AddAsync(item);
        }

        public async void Delete(int id)
        {
            T entity = await db.Set<T>().FindAsync(id);
            if (entity != null)
                db.Set<T>().Remove(entity);
        }

        public async Task<T> Get(int id)
        {
            return await db.Set<T>().FindAsync(id);
        }

        public IEnumerable<T> GetAll()
        {
            return db.Set<T>().ToList();
        }

        public void Update(T item)
        {
            db.Set<T>().Update(item);
        }
    }
}