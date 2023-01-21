using Filmofil.Domain.Entities;
using Filmofil.Neo4J;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filmofil.Services.Repositories.Comments
{
    public class CommentRepository : Neo4JRepository<Comment>, ICommentRepository
    {
        public CommentRepository(IGraphDatabase graphDatabase) : base(graphDatabase) { }

        protected override string NodeLabel => "Comment";

        public override async Task<long> CreateAsync(Comment comment)
        {
            byte[] gb = Guid.NewGuid().ToByteArray();
            comment.Id = BitConverter.ToInt32(gb, 0);

            await Graph.Cypher
                .Merge($"(author:User {{Id: {comment.AuthorId}}})")
                .With("author")
                .Merge($"(comment:Comment { GetDataOfComment(comment) })")
                .With("author, comment")
                .Merge($"(post:Post {{Id: {comment.PostId}}})")
                .Merge("(author)<-[:COMMENT_AUTHOR]-(comment)<-[:HAS_COMMENT]-(post)")
                .OnCreate()
                .Set("post.Popularity = post.Popularity + post.TimeStamp")
                .Merge("(comment)-[:MY_POST]->(post)")
                .ExecuteWithoutResultsAsync();

            return comment.Id;
        }

        public async Task<long> CreateReplyCommentAsync(Comment comment)
        {
            byte[] gb = Guid.NewGuid().ToByteArray();
            comment.Id = BitConverter.ToInt32(gb, 0);

            await Graph.Cypher
                .Merge($"(author:User {{Id: {comment.AuthorId}}})")
                .With("author")
                .Merge($"(comment:Comment { GetDataOfComment(comment) })")
                .With("author, comment")
                .Merge($"(post:Post {{Id: {comment.PostId}}})")
                .Merge("(author)<-[:COMMENT_AUTHOR]-(comment)-[:MY_POST]->(post)")
                .OnCreate()
                .Set("post.Popularity = post.Popularity + post.TimeStamp")
                .Merge($"(parent:Comment {{Id: {comment.ParentCommentId}}})")
                .With("comment, parent")
                .Merge("(comment)-[:PARENT_COMMENT]->(parent)")
                .Merge("(parent)-[:REPLY]->(comment)")
                .ExecuteWithoutResultsAsync();

            return comment.Id;
        }

        private string GetDataOfComment(Comment comment)
        {
            string attributs = "{" + $"Id: {comment.Id}," +
                             $"Content: \"{comment.Content}\"," +
                             $"LikesCount: {comment.LikesCount}" + "}";
            return attributs;
        }

        public async Task<List<Comment>> FindAllCommentsOfPostsAsync(List<long> postsIds)
        {
            return (await Graph.Cypher
                .Match("(comment:Comment)-[:MY_POST]->(myPost:Post)")
                .Where("myPost.Id IN { param }")
                .WithParam("param", postsIds)
                .OptionalMatch("(comment:Comment)-[:COMMENT_AUTHOR]-(author:User)")
                .OptionalMatch("(comment:Comment)<-[:COMMENT_LIKE]-(userLike:User)")
                .OptionalMatch("(comment:Comment)<-[:COMMENT_DISLIKE]-(userDislike:User)")
                .OptionalMatch("(comment:Comment)-[:PARENT_COMMENT]->(commParent:Comment)")
                .OptionalMatch("(comment:Comment)-[:REPLY]->(comments:Comment)")
                .Return((comment, myPost, author, userLike, userDislike, commParent, comments) =>
                    new
                    {
                        comment.As<Comment>().Id,
                        AuthorId = author.As<User>().Id,
                        PostId = myPost.As<Post>().Id,
                        ParentCommentId = commParent.As<Comment>().Id,
                        comment.As<Comment>().Content,
                        Likes = userLike.CollectAsDistinct<User>(),
                        Dislikes = userDislike.CollectAsDistinct<User>(),
                        comment.As<Comment>().LikesCount,
                        Comments = comments.CollectAsDistinct<Comment>()
                    }
                ).ResultsAsync).Select(myObject => new Comment()
                {
                    Id = myObject.Id,
                    AuthorId = myObject.AuthorId,
                    PostId = myObject.PostId,
                    ParentCommentId = myObject.ParentCommentId,
                    Content = myObject.Content,
                    Likes = myObject.Likes.ToList().ConvertAll(user => user.Id).ToList(),
                    Dislikes = myObject.Dislikes.ToList().ConvertAll(user => user.Id).ToList(),
                    LikesCount = myObject.LikesCount,
                    Comments = myObject.Comments.ToList().ConvertAll(comment => comment.Id).ToList()
                }).ToList();
        }

        public override async Task<Comment> FindAsync(long id)
        {
            return (await Graph.Cypher
                .Match("(comment:Comment)")
                .Where((Comment comment) => comment.Id == id)
                .OptionalMatch("(comment:Comment)-[:COMMENT_AUTHOR]-(author:User)")
                .OptionalMatch("(comment:Comment)-[:MY_POST]->(myPost:Post)")
                .OptionalMatch("(comment:Comment)<-[:COMMENT_LIKE]-(userLike:User)")
                .OptionalMatch("(comment:Comment)<-[:COMMENT_DISLIKE]-(userDislike:User)")
                .OptionalMatch("(comment:Comment)-[:PARENT_COMMENT]->(commParent:Comment)")
                .OptionalMatch("(comment:Comment)-[:REPLY]->(comments:Comment)")
                .Return((comment, author, myPost, userLike, userDislike, comments, commParent) =>
                    new
                    {
                        comment.As<Comment>().Id,
                        AuthorId = author.As<User>().Id,
                        PostId = myPost.As<Post>().Id,
                        ParentCommentId = commParent.As<Comment>().Id,
                        comment.As<Comment>().Content,
                        Likes = userLike.CollectAsDistinct<User>(),
                        Dislikes = userDislike.CollectAsDistinct<User>(),
                        comment.As<Comment>().LikesCount,
                        Comments = comments.CollectAsDistinct<Comment>()
                    }
                )
                .ResultsAsync).Select(myObject => new Comment()
                {
                    Id = myObject.Id,
                    AuthorId = myObject.AuthorId,
                    PostId = myObject.PostId,
                    ParentCommentId = myObject.ParentCommentId,
                    Content = myObject.Content,
                    Likes = myObject.Likes.ToList().ConvertAll(user => user.Id).ToList(),
                    Dislikes = myObject.Dislikes.ToList().ConvertAll(user => user.Id).ToList(),
                    LikesCount = myObject.LikesCount,
                    Comments = myObject.Comments.ToList().ConvertAll(comment => comment.Id).ToList()
                }).FirstOrDefault();
        }

        public async Task DislikeCommentAsync(long commentId, long userId)
        {
            await Graph.Cypher
               .Match($"(comment:Comment {{Id: {commentId}}}), (user:User {{Id: {userId}}})")
               .OptionalMatch("(comment:Comment)-[r]-(user:User)")
               .Where("type(r)= 'COMMENT_DISLIKE' or type(r)= 'COMMENT_LIKE'")
               .ForEach("(p in case when type(r)= 'COMMENT_LIKE' then[1] else [] end | " +
                        "delete(r) create((comment)<-[:COMMENT_DISLIKE]-(user)) set comment.LikesCount = comment.LikesCount - 2)")
               .ForEach("(p in case when type(r)= 'COMMENT_DISLIKE' then[1] else [] end | " +
                        "delete(r) set comment.LikesCount = comment.LikesCount + 1)")
               .ForEach("(p in case when r is null then[1] else [] end | " +
                        "create((comment)<-[:COMMENT_DISLIKE]-(user)) set comment.LikesCount = comment.LikesCount - 1)")
               .ExecuteWithoutResultsAsync();
        }

        public async Task LikeCommentAsync(long commentId, long userId)
        {
            await Graph.Cypher
               .Match($"(comment:Comment {{Id: {commentId}}}), (user:User {{Id: {userId}}})")
               .OptionalMatch("(comment:Comment)-[r]-(user:User)")
               .Where("type(r)= 'COMMENT_DISLIKE' or type(r)= 'COMMENT_LIKE'")
               .ForEach("(p in case when type(r)= 'COMMENT_DISLIKE' then[1] else [] end | " +
                        "delete(r) create((comment)<-[:COMMENT_LIKE]-(user)) set comment.LikesCount = comment.LikesCount + 2)")
               .ForEach("(p in case when type(r)= 'COMMENT_LIKE' then[1] else [] end | " +
                        "delete(r) set comment.LikesCount = comment.LikesCount - 1)")
               .ForEach("(p in case when r is null then[1] else [] end | " +
                        "create((comment)<-[:COMMENT_LIKE]-(user)) set comment.LikesCount = comment.LikesCount + 1)")
               .ExecuteWithoutResultsAsync();
        }
    }
}
