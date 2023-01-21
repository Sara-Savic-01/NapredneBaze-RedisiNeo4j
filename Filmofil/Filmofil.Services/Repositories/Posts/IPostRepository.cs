using Filmofil.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Filmofil.Services.Repositories.Posts
{
    public interface IPostRepository : INeo4JRepository<Post>
    {
        Task<List<Post>> FindManyPostsAsync(List<long> showedPostsIds, int num, string category);
        Task<List<Post>> FindMorePostsOfCategoriesAsync(List<long> showedPostsIds, int num, string category, List<string> categorieTitles);
        Task<List<Post>> GetPostsOfUserAsync(long userId);
        Task DislikePostAsync(long postId, long userId);
        Task LikePostAsync(long postId, long userId);
    }
}
