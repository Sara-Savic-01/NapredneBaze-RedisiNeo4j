using Filmofil.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Filmofil.Services.Repositories.Comments
{
    public interface ICommentRepository : INeo4JRepository<Comment>
    {
        Task<long> CreateReplyCommentAsync(Comment comment);
        Task DislikeCommentAsync(long commentId, long userId);
        Task LikeCommentAsync(long commentId, long userId);
        Task<List<Comment>> FindAllCommentsOfPostsAsync(List<long> postsIds);
    }
}
