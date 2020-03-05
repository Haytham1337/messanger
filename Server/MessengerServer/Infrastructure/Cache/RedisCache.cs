using Application;
using Microsoft.Extensions.Caching.Distributed;
using System;

namespace Infrastructure.Cache
{
    public class RedisCache : ICache
    {
        private IDistributedCache _cache;

        public RedisCache(IDistributedCache cache)
        {
            _cache = cache;
        }
        public object Get(object key)
        {
            return _cache.GetString((string)key);
        }

        public void Set(object key, object value, TimeSpan expireTime)
        {
            _cache.SetString((string)key, (string)value, new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = expireTime
            });
        }
    }
}
