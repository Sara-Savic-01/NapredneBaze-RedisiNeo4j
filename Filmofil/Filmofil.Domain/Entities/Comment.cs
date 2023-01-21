using System.Collections.Generic;

namespace Filmofil.Domain.Entities
{
    public class Comment : IEntity
    {
        public long Id { get; set; }
        public long AuthorId { get; set; }
        public long PostId { get; set; }
        public long? ParentCommentId { get; set; }
        public string Content { get; set; }
        public List<long> Likes { get; set; }
        public List<long> Dislikes { get; set; }
        public int LikesCount { get; set; }
        public List<long> Comments { get; set; }
    }
}
