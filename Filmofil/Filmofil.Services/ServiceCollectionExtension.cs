using Microsoft.Extensions.DependencyInjection;
using Filmofil.Services.Repositories;
using Filmofil.Services.Repositories.Chat;
using Filmofil.Services.Repositories.Comments;
using Filmofil.Services.Repositories.Categories;
using Filmofil.Services.Repositories.Posts;
using Filmofil.Services.Repositories.Users;
using Filmofil.Services.Services;

namespace Filmofil.Services
{
    public static class ServiceCollectionExtension
    {
        public static void AddFilmofilServices(this IServiceCollection services)
        {
            services.AddScoped<IRedisRepository, RedisRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ICategorieRepository, CategorieRepository>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IChatRepository, ChatRepository>();
        }
    }
}
