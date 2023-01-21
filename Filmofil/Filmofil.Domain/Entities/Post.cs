using System.Collections.Generic;

namespace Filmofil.Domain.Entities
{
    public class Post : IEntity
    {
        public long Id { get; set; }
        public long AuthorId { get; set; }
        public string Content { get; set; }
        public List<long> Likes { get; set; }
        public List<long> Dislikes { get; set; }
        public int LikesCount { get; set; }
        public List<long> Comments { get; set; }
        public long Categorie { get; set; }
        public string CategorieTitle { get; set; }
        public double TimeStamp { get; set; }
        public double Popularity { get; set; }
    }
}
