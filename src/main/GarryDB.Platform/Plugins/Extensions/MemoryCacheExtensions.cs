using System;
using System.Runtime.Caching;

namespace GarryDb.Platform.Plugins.Extensions
{
    internal static class MemoryCacheExtensions
    {
        private static readonly CacheItemPolicy CacheItemPolicy = new CacheItemPolicy
        {
            SlidingExpiration = TimeSpan.FromMinutes(1)
        };

        public static T? GetOrAdd<T>(this MemoryCache cache, string key, Func<T?> valueFactory)
        {
            if (cache.Contains(key))
            {
                var cachedAssembly = (CachedResult)cache[key];
                return (T?)cachedAssembly.Result;
            }

            T? result = valueFactory();
            cache.Add(key, new CachedResult(result), CacheItemPolicy);
            return result;
        }
       
        private readonly struct CachedResult
        {
            public CachedResult(object? result)
            {
                Result = result;
            }

            public object? Result { get; }
        }
    }
}