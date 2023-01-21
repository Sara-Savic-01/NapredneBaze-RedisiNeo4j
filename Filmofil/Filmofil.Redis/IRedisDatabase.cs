using StackExchange.Redis;

namespace Filmofil.Redis
{
    public interface IRedisDatabase
    {
        IDatabase RedisDB { get; }
        ISubscriber PubSub { get; }
    }
}
