using Filmofil.Domain.Entities;
using System.Collections.Generic;

namespace Filmofil.Business.ReduxLoaderManagement
{
    public class AppState
    {
        public AppState()
        {
            Posts = new List<Post>();
            Comments = new List<Comment>();
            Users = new List<User>();
        }

        public List<Categorie>? Categories { get; set; }
        public List<Post> Posts { get; set; }
        public List<Comment> Comments { get; set; }
        public List<User> Users { get; set; }
    }
}
