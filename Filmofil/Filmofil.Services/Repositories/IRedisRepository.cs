using Filmofil.Domain.Entities;
using System.Threading.Tasks;

namespace Filmofil.Services.Repositories
{
    public interface IRedisRepository
    {
        Task<T> GetAsync<T>(string key);
        Task<bool> DeleteAsync<T>(string key);
        Task<bool> SetAsync<T>(string key, T value);
        Task Subscribe(string topic, string subscriber);
        Task Publish(Message message);
    }
}
