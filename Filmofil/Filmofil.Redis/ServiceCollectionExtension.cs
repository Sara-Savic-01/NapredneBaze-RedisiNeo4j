using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Filmofil.Redis
{
    public static class ServiceCollectionExtension
    {
        public static void AddRedisServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IRedisDatabase, RedisDatabase>();
            services.Configure<RedisOptions>(configuration.GetSection("Redis"));
        }
    }
}
