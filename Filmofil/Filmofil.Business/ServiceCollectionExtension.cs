using Microsoft.Extensions.DependencyInjection;
using Filmofil.Business.ChatManagement;
using Filmofil.Business.CommentManagement;
using Filmofil.Business.CategorieManagment;
using Filmofil.Business.PostManagement;
using Filmofil.Business.ReduxLoaderManagement;
using Filmofil.Business.UserManagement;

namespace Filmofil.Business
{
    public static class ServiceCollectionExtension
    {
        public static void AddBussinesServices(this IServiceCollection services)
        {
            services.AddScoped<IReduxLoaderManager, ReduxLoaderManager>();
            services.AddScoped<IPostManager, PostManager>();
            services.AddScoped<ICommentManager, CommentManager>();
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<ICategorieManager, CategorieManager>();
            services.AddScoped<IChatManager, ChatManager>();
        }
    }
}
