using System.Collections.Generic;

namespace Filmofil.Domain.Entities
{
    public class User : IEntity
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<long> Posts { get; set; }
        public List<long> Comments { get; set; }
        public List<long> LikedPosts { get; set; }
        public List<long> DislikedPosts { get; set; }
        public List<long> LikedComments { get; set; }
        public List<long> DislikedComments { get; set; }
        public List<long> Categories { get; set; }
        public List<long> Messages { get; set; }

        public List<long> Followers { get; set; }

        public List<long> Following { get; set; }
    }
}
