using Filmofil.Business.PostManagement.Input;
using Filmofil.Domain.Interop;
using System.Threading.Tasks;

namespace Filmofil.Business.PostManagement
{
    public interface IPostManager
    {
        Task<Result<long>> AddPost(CreatePostInput input);

        Task<Result<long>> AddCommentToPost(CreateCommentInput input);

        Task<Result> LikePost(long postId, long userId, string categorieTitle);

        Task<Result> DislikePost(long postId, long userId, string categorieTitle);


    }
}
