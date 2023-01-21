using Filmofil.Business.CommentManagement.Input;
using Filmofil.Domain.Interop;
using System.Threading.Tasks;

namespace Filmofil.Business.CommentManagement
{
    public interface ICommentManager
    {
        Task<Result<long>> ReplyToComment(CreateReplyCommentInput input);

        Task<Result> LikeComment(long commentId, long userId, string categorieTitle);

        Task<Result> DislikeComment(long commentId, long userId, string categorieTitle);
    }
}
