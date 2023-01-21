using Filmofil.Domain.Entities;

namespace Filmofil.Services.Services
{
    public interface IPostService
    {
        public int ComparePosts(Post firstPost, Post secondPost);

        public double PostPopularity(Post post);
    }
}
