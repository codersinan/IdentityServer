using CacheServer.Interfaces;

namespace RedisCacheServer
{
    public class RedisCacheConfiguration : ICacheServerConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Instance { get; set; }
        public int Ttl { get; set; }
    }
}