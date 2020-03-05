using System;

namespace Application
{
    public interface ICache
    {
        object Get(object key);

        void Set(object key, object value, TimeSpan expireTime);
    }
}
