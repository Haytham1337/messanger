using Application;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Infrastructure
{
    public class MemoryCache : ICache
    {
        private readonly IMemoryCache _cache;

        public MemoryCache(IMemoryCache cache)
        {
            _cache = cache;
        }
        public object Get(object key)
        {
            return _cache.Get(key);
        }

        public void Set(object key, object value, TimeSpan expireTime)
        {
            _cache.Set(key, value, new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = expireTime
            });
        }
    }
}
