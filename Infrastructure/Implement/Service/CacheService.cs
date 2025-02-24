using Application.Interface.Service;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace Infrastructure.Implement.Service
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache cache;

        public CacheService(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public T? Get<T>(string key)
        {
            cache.TryGetValue(key, out string? cacheData);
            if (cacheData != null) return JsonSerializer.Deserialize<T>(cacheData);
            return default;
        }

        public void Set<T>(string key, T data, int minutesValid)
        {
            var dataJson = JsonSerializer.Serialize(data);
            cache.Set(key, dataJson, TimeSpan.FromMinutes(minutesValid));
        }
    }
}
