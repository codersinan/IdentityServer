using CacheServer.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RedisCacheServer
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRedisDistributedCache(this IServiceCollection services, IConfiguration configuration)
        {
            var cacheConfiguration = new RedisCacheConfiguration();
            configuration.Bind(nameof(RedisCacheConfiguration), cacheConfiguration);

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = $"{cacheConfiguration.Host}:{cacheConfiguration.Port}";
                options.InstanceName = $"{cacheConfiguration.Instance}";
            });

            services.AddTransient<ICacheRepository, RedisCacheRepository>();
        }
    }
}