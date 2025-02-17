using Application.Interface.Service;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Application.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache cache;

        public CacheService(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var cacheData = await cache.GetStringAsync(key);
            if (cacheData != null) return JsonSerializer.Deserialize<T>(cacheData);
            return default;
        }

        public async Task SetAsync<T>(string key, T data, int minutesValid)
        {
            var dataJson = JsonSerializer.Serialize(data);
            var opts = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(minutesValid));
            await cache.SetStringAsync(key, dataJson, opts);
        }
    }
}
