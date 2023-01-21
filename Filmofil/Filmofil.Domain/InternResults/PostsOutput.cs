using Filmofil.Domain.Entities;
using System.Collections.Generic;

namespace Filmofil.Domain.InternResults
{
    public class PostsOutput
    {
        public List<Post> Posts { get; set; }
        public List<Comment> PostsComments { get; set; }
        public List<User> Authors { get; set; }

    }
}
