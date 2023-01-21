using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Filmofil.Redis
{
    public class RedisDatabase : IRedisDatabase
    {
        public IDatabase RedisDB { get; }
        public ISubscriber PubSub { get; }

        public RedisDatabase(IOptions<RedisOptions> options)
        {
            ConfigurationOptions configurationOptions = new ConfigurationOptions
            {
                EndPoints = { { "https://localhost", 6379 } },
                ClientName = options.Value.ClientName
            };

            IConnectionMultiplexer connectionMultiplexer =
                ConnectionMultiplexer.Connect("localhost");

            PubSub = connectionMultiplexer.GetSubscriber();
            RedisDB = connectionMultiplexer.GetDatabase();
        }
    }
}
