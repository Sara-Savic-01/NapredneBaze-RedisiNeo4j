using Newtonsoft.Json;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Filmofil.Redis
{
    public static class RedisDatabaseExtension
    {
        public async static Task<bool> Set<T>(this IDatabase database, string key, T value)
        {
            return await database.StringSetAsync(key, JsonConvert.SerializeObject(value));
        }

        public async static Task<T> Get<T>(this IDatabase database, string key)
        {
            RedisValue result = await database.StringGetAsync(key);
            if (result.HasValue)
                return JsonConvert.DeserializeObject<T>(result);
            return default;
        }
    }
}
