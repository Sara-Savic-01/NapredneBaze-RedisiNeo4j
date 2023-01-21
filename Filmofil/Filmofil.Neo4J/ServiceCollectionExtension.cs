using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Filmofil.Neo4J
{
    public static class ServiceCollectionExtension
    {
        public static void AddNeo4JServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IGraphDatabase, GraphDatabase>();
            services.Configure<Neo4JOptions>(configuration.GetSection("Neo4J"));
        }
    }
}
