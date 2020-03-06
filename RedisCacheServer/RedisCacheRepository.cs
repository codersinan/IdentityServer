using System;
using System.Text;
using System.Threading.Tasks;
using CacheServer.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace RedisCacheServer
{
    public class RedisCacheRepository : ICacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly DistributedCacheEntryOptions _options;

        public RedisCacheRepository(IDistributedCache distributedCache, IConfiguration configuration)
        {
            _distributedCache = distributedCache;
            var cacheConfiguration = new RedisCacheConfiguration();
            configuration.Bind(nameof(RedisCacheConfiguration), cacheConfiguration);

            _options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(cacheConfiguration.Ttl < 0 ? cacheConfiguration.Ttl : 5)
            };
        }

        public void Set<T>(string key, T data)
        {
            var serialize = JsonConvert.SerializeObject(data);
            var bytes = Encoding.UTF8.GetBytes(serialize);
            _distributedCache.Set(key, bytes, _options);
        }

        public async Task SetAsync<T>(string key, T data)
        {
            var serialize = JsonConvert.SerializeObject(data);
            var bytes = Encoding.UTF8.GetBytes(serialize);
            await _distributedCache.SetAsync(key, bytes, _options);
        }

        public T Read<T>(string key)
        {
            var json = _distributedCache.GetString(key);
            return json != null ? JsonConvert.DeserializeObject<T>(json) : default(T);
        }

        public async Task<T> ReadAsync<T>(string key)
        {
            var json = await _distributedCache.GetStringAsync(key);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}