using System.Collections.Generic;

namespace Filmofil.Business.PostManagement.Input
{
    public class MorePostsInput
    {
        public string Category { get; set; }
        public List<long> PostsIds { get; set; }
        public long UserId { get; set; }
        public string Categorie { get; set; }
    }
}
