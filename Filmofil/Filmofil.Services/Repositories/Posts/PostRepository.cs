using Filmofil.Domain.Entities;
using Filmofil.Domain.Entities.Enums;
using Filmofil.Neo4J;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filmofil.Services.Repositories.Posts
{
    public class PostRepository : Neo4JRepository<Post>, IPostRepository
    {
        public PostRepository(IGraphDatabase graphDatabase) : base(graphDatabase) { }

        protected override string NodeLabel => "Post";

        public override async Task<long> CreateAsync(Post post)
        {
            byte[] gb = Guid.NewGuid().ToByteArray();
            post.Id = BitConverter.ToInt32(gb, 0);

            await Graph.Cypher
                .Merge($"(author:User {{Id: {post.AuthorId}}})")
                .With("author")
                .Merge($"(post:Post { GetPostData(post) })")
                .With("author, post")
                .Merge($"(categorie:Categorie {{Id: {post.Categorie}}})")
                .Merge("(author)<-[:POST_AUTHOR]-(post)<-[:HAS_POST]-(categorie)")
                .ExecuteWithoutResultsAsync();

            return post.Id;
        }

        private string GetPostData(Post post)
        {
            return "{" + $"Id: {post.Id}," +
                $"Content: \"{post.Content}\"," +
                $"CategorieTitle: \"{post.CategorieTitle}\"," +
                $"LikesCount: {post.LikesCount}," +
                $"TimeStamp: {post.TimeStamp}," +
                $"Popularity: {post.Popularity}" + "}";
        }

        public async Task<List<Post>> FindManyPostsAsync(List<long> showedPostsIds, int num, string category)
        {
            string attribute = GetAttribute(category);

            return (await Graph.Cypher
                .Match("(categorie:Categorie)-[:HAS_POST]->(post:Post)-[:POST_AUTHOR]->(postAuthor:User)")
                .Where("NOT post.Id IN { param }")
                .WithParam("param", showedPostsIds)
                .OptionalMatch("(post:Post)<-[:POST_LIKE]-(userLike:User)")
                .OptionalMatch("(post:Post)<-[:POST_DISLIKE]-(userDislike:User)")
                .OptionalMatch("(post:Post)-[:HAS_COMMENT]->(comment:Comment)")
                .Return((categorie, post, postAuthor, userLike, userDislike, comment) =>
                    new
                    {
                        post.As<Post>().Id,
                        AuthorId = postAuthor.As<User>().Id,
                        post.As<Post>().Content,
                        Likes = userLike.CollectAsDistinct<User>(),
                        Dislikes = userDislike.CollectAsDistinct<User>(),
                        post.As<Post>().LikesCount,
                        Comments = comment.CollectAsDistinct<Comment>(),
                        Categorie = categorie.As<Categorie>().Id,
                        CategorieTitle = categorie.As<Categorie>().Title,
                        post.As<Post>().TimeStamp,
                        post.As<Post>().Popularity
                    }
                )
                .OrderBy("post." + attribute + " DESC")
                .Limit(num)
                .ResultsAsync).Select(myObject => new Post()
                {
                    Id = myObject.Id,
                    AuthorId = myObject.AuthorId,
                    Content = myObject.Content,
                    Likes = myObject.Likes.ToList().ConvertAll(user => user.Id).ToList(),
                    Dislikes = myObject.Dislikes.ToList().ConvertAll(user => user.Id).ToList(),
                    LikesCount = myObject.LikesCount,
                    Comments = myObject.Comments.ToList().ConvertAll(comment => comment.Id).ToList(),
                    Categorie = myObject.Categorie,
                    CategorieTitle = myObject.CategorieTitle,
                    TimeStamp = myObject.TimeStamp,
                    Popularity = myObject.Popularity
                }).ToList();
        }

        private string GetAttribute(string category)
        {
            string att = "TimeStamp";

            if (category.Equals(RedisKeys.POPULAR_POSTS))
                att = "Popularity";
            else if (category.Equals(RedisKeys.BEST_POSTS))
                att = "LikesCount";
            return att;
        }

        public async Task<List<Post>> FindMorePostsOfCategoriesAsync(List<long> showedPostsIds, int num, string category, List<string> categorieTitles)
        {
            string attribute = GetAttribute(category);

            return (await Graph.Cypher
               .Match("(categorie:Categorie)-[:HAS_POST]->(post:Post)-[:POST_AUTHOR]->(postAuthor:User)")
               .Where("categorie.Title IN { categorieParam }")
               .WithParam("categorieParam", categorieTitles)
               .AndWhere("NOT post.Id IN { param }")
               .WithParam("param", showedPostsIds)
               .OptionalMatch("(post:Post)<-[:POST_LIKE]-(userLike:User)")
               .OptionalMatch("(post:Post)<-[:POST_DISLIKE]-(userDislike:User)")
               .OptionalMatch("(post:Post)-[:HAS_COMMENT]->(comment:Comment)")
               .Return((categorie, post, postAuthor, userLike, userDislike, comment) =>
                   new
                   {
                       post.As<Post>().Id,
                       AuthorId = postAuthor.As<User>().Id,
                       post.As<Post>().Content,
                       Likes = userLike.CollectAsDistinct<User>(),
                       Dislikes = userDislike.CollectAsDistinct<User>(),
                       post.As<Post>().LikesCount,
                       Comments = comment.CollectAsDistinct<Comment>(),
                       Categorie = categorie.As<Categorie>().Id,
                       CategorieTitle = categorie.As<Categorie>().Title,
                       post.As<Post>().TimeStamp,
                       post.As<Post>().Popularity
                   }
               )
               .OrderBy("post." + attribute + " DESC")
               .Limit(num)
               .ResultsAsync).Select(myObject => new Post()
               {
                   Id = myObject.Id,
                   AuthorId = myObject.AuthorId,
                   Content = myObject.Content,
                   Likes = myObject.Likes.ToList().ConvertAll(user => user.Id).ToList(),
                   Dislikes = myObject.Dislikes.ToList().ConvertAll(user => user.Id).ToList(),
                   LikesCount = myObject.LikesCount,
                   Comments = myObject.Comments.ToList().ConvertAll(comment => comment.Id).ToList(),
                   Categorie = myObject.Categorie,
                   CategorieTitle = myObject.CategorieTitle,
                   TimeStamp = myObject.TimeStamp,
                   Popularity = myObject.Popularity
               }).ToList();
        }

        public async Task<List<Post>> GetPostsOfUserAsync(long userId)
        {
            return (await Graph.Cypher
                .Match("(categorie:Categorie)-[:HAS_POST]->(post:Post)-[:POST_AUTHOR]->(postAuthor:User)")
                .Where("postAuthor.Id = { param }")
                .WithParam("param", userId)
                .OptionalMatch("(post:Post)<-[:POST_LIKE]-(userLike:User)")
                .OptionalMatch("(post:Post)<-[:POST_DISLIKE]-(userDislike:User)")
                .OptionalMatch("(post:Post)-[:HAS_COMMENT]->(comment:Comment)")
                .Return((categorie, post, postAuthor, userLike, userDislike, comment) =>
                    new
                    {
                        post.As<Post>().Id,
                        AuthorId = postAuthor.As<User>().Id,
                        post.As<Post>().Content,
                        Likes = userLike.CollectAsDistinct<User>(),
                        Dislikes = userDislike.CollectAsDistinct<User>(),
                        post.As<Post>().LikesCount,
                        Comments = comment.CollectAsDistinct<Comment>(),
                        Categorie = categorie.As<Categorie>().Id,
                        CategorieTitle = categorie.As<Categorie>().Title,
                        post.As<Post>().TimeStamp,
                        post.As<Post>().Popularity
                    }
                )
                .ResultsAsync).Select(myObject => new Post()
                {
                    Id = myObject.Id,
                    AuthorId = myObject.AuthorId,
                    Content = myObject.Content,
                    Likes = myObject.Likes.ToList().ConvertAll(user => user.Id).ToList(),
                    Dislikes = myObject.Dislikes.ToList().ConvertAll(user => user.Id).ToList(),
                    LikesCount = myObject.LikesCount,
                    Comments = myObject.Comments.ToList().ConvertAll(comment => comment.Id).ToList(),
                    Categorie = myObject.Categorie,
                    CategorieTitle = myObject.CategorieTitle,
                    TimeStamp = myObject.TimeStamp,
                    Popularity = myObject.Popularity
                }).ToList();
        }

        public override async Task<Post> FindAsync(long id)
        {
            return (await Graph.Cypher
                .Match("(post:Post)")
                .Where((Post post) => post.Id == id)
                .OptionalMatch("(post:Post)-[:POST_AUTHOR]-(user:User)")
                .OptionalMatch("(post:Post)-[:HAS_COMMENT]-(comments:Comment)")
                .OptionalMatch("(post:Post)-[:POST_LIKE]-(userLike:User)")
                .OptionalMatch("(post:Post)-[:POST_DISLIKE]-(userDislike:User)")
                .OptionalMatch("(post:Post)-[:HAS_POST]-(categorie:Categorie)")
                .Return((post, user, userLike, userDislike, comments, categorie) =>
                    new
                    {
                        post.As<Post>().Id,
                        AuthorId = user.As<User>().Id,
                        post.As<Post>().Content,
                        Likes = userLike.CollectAsDistinct<User>(),
                        Dislikes = userDislike.CollectAsDistinct<User>(),
                        post.As<Post>().LikesCount,
                        Comments = comments.CollectAsDistinct<Comment>(),
                        Categorie = categorie.As<Categorie>().Id,
                        CategorieTitle = categorie.As<Categorie>().Title,
                        post.As<Post>().TimeStamp,
                        post.As<Post>().Popularity
                    }
                )
                .ResultsAsync).Select(myObject => new Post()
                {
                    Id = myObject.Id,
                    AuthorId = myObject.AuthorId,
                    Content = myObject.Content,
                    Likes = myObject.Likes.ToList().ConvertAll(user => user.Id).ToList(),
                    Dislikes = myObject.Dislikes.ToList().ConvertAll(user => user.Id).ToList(),
                    LikesCount = myObject.LikesCount,
                    Comments = myObject.Comments.ToList().ConvertAll(comment => comment.Id).ToList(),
                    Categorie = myObject.Categorie,
                    CategorieTitle = myObject.CategorieTitle,
                    TimeStamp = myObject.TimeStamp,
                    Popularity = myObject.Popularity
                })
                .FirstOrDefault();
        }

        public async Task DislikePostAsync(long postId, long userId)
        {
            await Graph.Cypher
               .Match($"(post:Post {{Id: {postId}}}), (user:User {{Id: {userId}}})")
               .OptionalMatch("(post:Post)-[r]-(user:User)")
               .Where("type(r)= 'POST_DISLIKE' or type(r)= 'POST_LIKE'")
               .ForEach("(p in case when type(r)= 'POST_LIKE' then[1] else [] end | " +
                        "delete(r) create((post)<-[:POST_DISLIKE]-(user)) set post.LikesCount = post.LikesCount - 2)")
               .ForEach("(p in case when type(r)= 'POST_DISLIKE' then[1] else [] end | " +
                        "delete(r) set post.LikesCount = post.LikesCount + 1, post.Popularity = post.Popularity - post.TimeStamp)")
               .ForEach("(p in case when r is null then[1] else [] end | " +
                        "create((post)<-[:POST_DISLIKE]-(user)) set post.LikesCount = post.LikesCount - 1, post.Popularity = post.Popularity + post.TimeStamp)")
               .ExecuteWithoutResultsAsync();
        }

        public async Task LikePostAsync(long postId, long userId)
        {
            await Graph.Cypher
               .Match($"(post:Post {{Id: {postId}}}), (user:User {{Id: {userId}}})")
               .OptionalMatch("(post:Post)-[r]-(user:User)")
               .Where("type(r)= 'POST_DISLIKE' or type(r)= 'POST_LIKE'")
               .ForEach("(p in case when type(r)= 'POST_DISLIKE' then[1] else [] end | " +
                        "delete(r) create((post)<-[:POST_LIKE]-(user)) set post.LikesCount = post.LikesCount + 2)")
               .ForEach("(p in case when type(r)= 'POST_LIKE' then[1] else [] end | " +
                        "delete(r) set post.LikesCount = post.LikesCount - 1, post.Popularity = post.Popularity - post.TimeStamp)")
               .ForEach("(p in case when r is null then[1] else [] end | " +
                        "create((post)<-[:POST_LIKE]-(user)) set post.LikesCount = post.LikesCount + 1, post.Popularity = post.Popularity + post.TimeStamp)")
               .ExecuteWithoutResultsAsync();
        }
    }

    public class ResultsN4J
    {
        public Post ResPost { get; set; }
        public IEnumerable<Comment> ResComments { get; set; }
        public User ResPostAuthor { get; set; }
        public IEnumerable<User> ResUserCommAuthors { get; set; }
    }
}
