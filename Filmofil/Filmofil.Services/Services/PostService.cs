using Filmofil.Domain.Entities;

namespace Filmofil.Services.Services
{
    public class PostService : IPostService
    {
        public int ComparePosts(Post firstPost, Post secondPost)
        {
            double firstPopularity = PostPopularity(firstPost);
            double secondPopularity = PostPopularity(secondPost);

            if (firstPopularity > secondPopularity)
                return 1;
            else if (firstPopularity < secondPopularity)
                return -1;
            else
                return 0;
        }

        public double PostPopularity(Post post)
        {
            return (post.Likes.Count + post.Dislikes.Count + post.Comments.Count) * post.TimeStamp;
        }
    }
}
