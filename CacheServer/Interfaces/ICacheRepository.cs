using System.Threading.Tasks;

namespace CacheServer.Interfaces
{
    public interface ICacheRepository
    {
        void Set<T>(string key, T data);
        Task SetAsync<T>(string key, T data);

        T Read<T>(string key);
        Task<T> ReadAsync<T>(string key);
    }
}