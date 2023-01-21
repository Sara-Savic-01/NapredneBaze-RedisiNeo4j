using Filmofil.Domain.Entities;
using Filmofil.Neo4J;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filmofil.Services.Repositories.Users
{
    public class UserRepository : Neo4JRepository<User>, IUserRepository
    {
        public UserRepository(IGraphDatabase graphDatabase) : base(graphDatabase) { }

        protected override string NodeLabel => "User";

        public override async Task<long> CreateAsync(User user)
        {
            byte[] gb = Guid.NewGuid().ToByteArray();
            user.Id = BitConverter.ToInt32(gb, 0);

            await Graph.Cypher
                .Create("(node:" + NodeLabel + GetDataOfUser(user) + ")")
                .ExecuteWithoutResultsAsync();

            return user.Id;
        }

        private string GetDataOfUser(User user)
        {
            return "{" + $"Id: {user.Id}," +
                             $"Email: \"{user.Email}\"," +
                             $"Username: \"{user.Username}\"," +
                             $"Password: \"{user.Password}\"" + "}";
        }

        public override async Task<User> FindAsync(long id)
        {
            return (await Graph.Cypher
                .Match("(user:User)")
                .Where((User user) => user.Id == id)
                .OptionalMatch("(user:User)-[:POST_AUTHOR]-(post:Post)")
                .OptionalMatch("(user:User)-[:COMMENT_AUTHOR]-(comments:Comment)")
                .OptionalMatch("(user:User)-[:POST_LIKE]-(postLike:Post)")
                .OptionalMatch("(user:User)-[:POST_DISLIKE]-(postDislike:Post)")
                .OptionalMatch("(user:User)-[:COMMENT_LIKE]-(commentLike:Comment)")
                .OptionalMatch("(user:User)-[:COMMENT_DISLIKE]-(commentDislike:Comment)")
                .OptionalMatch("(user:User)-[:HAS_USER]-(categorie:Categorie)")
                .OptionalMatch("(user:User)-[:MESSAGE_AUTHOR]-(message:Message)")
                .Return((user, post, postLike, postDislike, comments,
                    commentLike, commentDislike, categorie, message) =>
                    new
                    {
                        user.As<User>().Id,
                        user.As<User>().Email,
                        user.As<User>().Username,
                        user.As<User>().Password,
                        Posts = post.CollectAsDistinct<Post>(),
                        Comments = comments.CollectAsDistinct<Comment>(),
                        LikedPosts = postLike.CollectAsDistinct<Post>(),
                        DislikedPosts = postDislike.CollectAsDistinct<Post>(),
                        LikedComments = commentLike.CollectAsDistinct<Comment>(),
                        DislikedComments = commentDislike.CollectAsDistinct<Comment>(),
                        Categories = categorie.CollectAsDistinct<Categorie>(),
                        Messages = message.CollectAsDistinct<Message>()
                    }
                ).ResultsAsync).Select(myObject => new User()
                {
                    Id = myObject.Id,
                    Email = myObject.Email,
                    Username = myObject.Username,
                    Password = myObject.Password,
                    Posts = myObject.Posts.ToList().ConvertAll(post => post.Id).ToList(),
                    Comments = myObject.Comments.ToList().ConvertAll(comment => comment.Id),
                    LikedPosts = myObject.LikedPosts.ToList().ConvertAll(post => post.Id),
                    DislikedPosts = myObject.DislikedPosts.ToList().ConvertAll(post => post.Id),
                    LikedComments = myObject.LikedComments.ToList().ConvertAll(comment => comment.Id),
                    DislikedComments = myObject.DislikedComments.ToList().ConvertAll(comment => comment.Id),
                    Categories = myObject.Categories.ToList().ConvertAll(cat=> cat.Id),
                    Messages = myObject.Messages.ToList().ConvertAll(mess => mess.Id)
                }).FirstOrDefault();
        }

        public async Task<List<User>> FindManyUsersAsync(List<long> userIds)
        {
            return (await Graph.Cypher
                .Match("(user:User)")
                .Where("user.Id IN { param }")
                .WithParam("param", userIds)
                .OptionalMatch("(user:User)-[:POST_AUTHOR]-(post:Post)")
                .OptionalMatch("(user:User)-[:COMMENT_AUTHOR]-(comments:Comment)")
                .OptionalMatch("(user:User)-[:POST_LIKE]-(postLike:Post)")
                .OptionalMatch("(user:User)-[:POST_DISLIKE]-(postDislike:Post)")
                .OptionalMatch("(user:User)-[:COMMENT_LIKE]-(commentLike:Comment)")
                .OptionalMatch("(user:User)-[:COMMENT_DISLIKE]-(commentDislike:Comment)")
                .OptionalMatch("(user:User)-[:HAS_USER]-(categorie:Categorie)")
                .OptionalMatch("(user:User)-[:MESSAGE_AUTHOR]-(message:Message)")
                .Return((user, post, postLike, postDislike, comments,
                    commentLike, commentDislike, categorie, message) =>
                    new
                    {
                        user.As<User>().Id,
                        user.As<User>().Email,
                        user.As<User>().Username,
                        user.As<User>().Password,
                        Posts = post.CollectAsDistinct<Post>(),
                        Comments = comments.CollectAsDistinct<Comment>(),
                        LikedPosts = postLike.CollectAsDistinct<Post>(),
                        DislikedPosts = postDislike.CollectAsDistinct<Post>(),
                        LikedComments = commentLike.CollectAsDistinct<Comment>(),
                        DislikedComments = commentDislike.CollectAsDistinct<Comment>(),
                        Categories = categorie.CollectAsDistinct<Categorie>(),
                        Messages = message.CollectAsDistinct<Message>()
                    }
                ).ResultsAsync).Select(myObject => new User()
                {
                    Id = myObject.Id,
                    Email = myObject.Email,
                    Username = myObject.Username,
                    Password = myObject.Password,
                    Posts = myObject.Posts.ToList().ConvertAll(post => post.Id).ToList(),
                    Comments = myObject.Comments.ToList().ConvertAll(comment => comment.Id),
                    LikedPosts = myObject.LikedPosts.ToList().ConvertAll(post => post.Id),
                    DislikedPosts = myObject.DislikedPosts.ToList().ConvertAll(post => post.Id),
                    LikedComments = myObject.LikedComments.ToList().ConvertAll(comment => comment.Id),
                    DislikedComments = myObject.DislikedComments.ToList().ConvertAll(comment => comment.Id),
                    Categories = myObject.Categories.ToList().ConvertAll(cat=> cat.Id),
                    Messages = myObject.Messages.ToList().ConvertAll(mess => mess.Id)
                }).ToList();
        }

        public async Task<User> GetUserByUsernameAsync(string username, string password)
        {
            return (await Graph.Cypher
                .Match("(node:" + NodeLabel + ")")
                .Where((User node) => node.Username == username && node.Password == password)
                .Return(node => node.As<User>()).ResultsAsync)
                .FirstOrDefault();
        }

        public async Task<bool> CheckUserByUsernameAsync(string username, string password)
        {
            return (await Graph.Cypher
                .Match("(node:" + NodeLabel + ")")
                .Where((User node) => node.Username == username && node.Password == password)
                .Return(node => node.As<User>()).ResultsAsync).Any();
        }
        //followerID je ID coveka koji prati drugog
        //followingID je ID coveka koji je pracen
        public async Task FollowUserAsync(long followerId, long followingId)
        {
            await Graph.Cypher
                .Match("(user1:User),(user2:User)")
                .Where((User user1) => user1.Id == followerId)
                .AndWhere((User user2) => user2.Id == followingId)
                .CreateUnique("(user1)-[:FOLLOWS]->(user2)")
                .ExecuteWithoutResultsAsync();

        }

        public async Task UnfollowUserAsync(long followerId, long followingId)
        {
            await Graph.Cypher
               .Match("(user2:User)<-[r:FOLLOWS]-(user1:User)")
               .Where((User user1) => user1.Id == followerId)
               .AndWhere((User user2) => user2.Id == followingId)
               .Delete("r")
               .ExecuteWithoutResultsAsync();

        }

        public async Task JoinToCategorieAsync(long categorieId, long userId)
        {
            await Graph.Cypher
                .Match("(user:User),(categorie:Categorie)")
                .Where((User user) => user.Id == userId)
                .AndWhere((Categorie categorie) => categorie.Id == categorieId)
                .CreateUnique("(categorie)-[:HAS_USER]->(user)")
                .ExecuteWithoutResultsAsync();
        }

        public async Task LeaveCategorieAsync(long categorieId, long userId)
        {
            await Graph.Cypher
               .Match("(user:User)<-[r:HAS_USER]-(categorie:Categorie)")
               .Where((User user) => user.Id == userId)
               .AndWhere((Categorie categorie) => categorie.Id == categorieId)
               .Delete("r")
               .ExecuteWithoutResultsAsync();
        }

    }
}
