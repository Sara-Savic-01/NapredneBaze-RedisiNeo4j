using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Filmofil.Domain.Entities;
using Filmofil.Redis;
using Filmofil.Services.Hubs;
using System.Threading.Tasks;

namespace Filmofil.Services.Repositories
{
    public class RedisRepository : IRedisRepository
    {
        private readonly IRedisDatabase _redis;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly string SendMessageCallback = "SendMessage";

        public RedisRepository(IRedisDatabase redis, IHubContext<MessageHub> hubContext)
        {
            _redis = redis;
            _hubContext = hubContext;
        }

        public async Task<bool> DeleteAsync<T>(string key)
        {
            return await _redis.RedisDB.KeyDeleteAsync(key);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            return await _redis.RedisDB.Get<T>(key);
        }

        public async Task<bool> SetAsync<T>(string key, T value)
        {
            return await _redis.RedisDB.Set(key, value);
        }

        public async Task Subscribe(string topic, string subscriber)
        {
            await _redis.PubSub.SubscribeAsync(topic, async (channel, message) =>
            {
                await _hubContext.Clients.Client(subscriber).SendAsync(
                    SendMessageCallback,
                    JsonConvert.DeserializeObject(message.ToString()));
            });
        }

        public async Task Publish(Message message)
        {
            await _redis.PubSub.PublishAsync(message.Categorie.ToString(),
                JsonConvert.SerializeObject(message));
        }
    }
}
