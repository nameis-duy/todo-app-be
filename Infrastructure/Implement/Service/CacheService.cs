using Application.Interface.Service;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Infrastructure.Implement.Service
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

        public async Task SetAsync<T>(string key, T data)
        {
            var dataJson = JsonSerializer.Serialize(data);
            await cache.SetStringAsync(key, dataJson);
        }
    }
}
