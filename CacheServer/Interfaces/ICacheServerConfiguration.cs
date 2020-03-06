namespace CacheServer.Interfaces
{
    public interface ICacheServerConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Instance { get; set; }
        public int Ttl { get; set; }
    }
}