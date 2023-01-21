using Filmofil.Domain.Interop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Filmofil.Business.ReduxLoaderManagement
{
    public interface IReduxLoaderManager
    {
        Task<Result<AppState>> LoadClientState(string category);
        Task<Result<AppState>> LoadClientState(string category, long userId);
        Task<Result<AppState>> LoadMorePosts(List<long> showedPostsIds, string category);
        Task<Result<AppState>> LoadMorePosts(List<long> showedPostsIds, string category, long userId);
        Task<Result<AppState>> LoadMorePostsOfCategorie(List<long> showedPostsIds, string category, string categorie);
    }
}
