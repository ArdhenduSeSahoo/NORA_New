using System;
using System.Collections.Concurrent;
using System.Runtime.Caching;
using System.Threading;

namespace Damco.Common
{
    public class InMemoryCache
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> lockingDictionary = new ConcurrentDictionary<string, SemaphoreSlim>();
        private static readonly MemoryCache _cache = MemoryCache.Default;

        public T GetOrCreate<T>(string key, Func<T> factory, CacheItemPolicy cachePolicy)
        {
            var result = _cache.Get(key);
            if (result != null)
            {
                return (T)result;
            }
            else
            {
                var semaphoreSlim = lockingDictionary.GetOrAdd(key, new SemaphoreSlim(1, 1));
                semaphoreSlim.Wait();
                try
                {
                    result = _cache.Get(key);
                    if (result != null)
                    {
                        return (T)result;
                    }
                    else
                    {
                        result = factory();
                        _cache.Set(key, result, cachePolicy);
                        return (T)result;
                    }
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            }
        }
    }
}
