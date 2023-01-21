using Filmofil.Business.CommentManagement.Input;
using Filmofil.Domain.Entities;
using Filmofil.Domain.Entities.Enums;
using Filmofil.Domain.Interop;
using Filmofil.Services.Repositories;
using Filmofil.Services.Repositories.Comments;
using Filmofil.Services.Repositories.Posts;
using Filmofil.Services.Repositories.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Filmofil.Business.CommentManagement
{
    public class CommentManager : ICommentManager
    {
        private IPostRepository _postRepository;
        private ICommentRepository _commentRepository;
        private IUserRepository _userRepository;
        private IRedisRepository _redisRepository;

        public CommentManager(IPostRepository postRepository,
          IUserRepository userRepository,
          IRedisRepository redisRepository,
          ICommentRepository commentRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _redisRepository = redisRepository;
            _commentRepository = commentRepository;
        }

        public async Task<Result> DislikeComment(long commentId, long userId, string categorieTitle)
        {
            await _commentRepository.DislikeCommentAsync(commentId, userId);

            string redisKey = categorieTitle + "-" + RedisKeys.COMMENTS;
            List<Comment> comments = await _redisRepository.GetAsync<List<Comment>>(redisKey);
            Comment commentToUpdate = comments.Find(com => com.Id == commentId);

            if (commentToUpdate != null)
            {
                if (commentToUpdate.Dislikes.Contains(userId))
                {
                    commentToUpdate.Dislikes.Remove(userId);
                    commentToUpdate.LikesCount += 1;
                }
                else if (!commentToUpdate.Likes.Contains(userId))
                {
                    commentToUpdate.Dislikes.Add(userId);
                    commentToUpdate.LikesCount -= 1;
                }
                else if (commentToUpdate.Likes.Contains(userId))
                {
                    commentToUpdate.Dislikes.Add(userId);
                    commentToUpdate.Likes.Remove(userId);
                    commentToUpdate.LikesCount -= 2;
                }

                await _redisRepository.SetAsync(redisKey, comments);
            }

            List<User> authors = await _redisRepository.GetAsync<List<User>>(
                RedisKeys.USERS).ConfigureAwait(false);

            User userToUpdate = authors.Find(x => x.Id == userId);

            if (userToUpdate != null)
            {
                if (userToUpdate.DislikedComments.Contains(commentId))
                {
                    userToUpdate.DislikedComments.Remove(commentId);
                }
                else if (!userToUpdate.LikedComments.Contains(commentId))
                {
                    userToUpdate.DislikedComments.Add(commentId);
                }
                else if (userToUpdate.LikedComments.Contains(commentId))
                {
                    userToUpdate.DislikedComments.Add(commentId);
                    userToUpdate.LikedComments.Remove(commentId);
                }

                await _redisRepository.SetAsync(RedisKeys.USERS, authors).ConfigureAwait(false);
            }

            return new Result() { Success = true };
        }

        public async Task<Result> LikeComment(long commentId, long userId, string categorieTitle)
        {
            await _commentRepository.LikeCommentAsync(commentId, userId).ConfigureAwait(false);

            string redisKey = categorieTitle + "-" + RedisKeys.COMMENTS;

            List<Comment> comments = await _redisRepository
                .GetAsync<List<Comment>>(redisKey).ConfigureAwait(false);

            Comment commentToUpdate = comments.Find(com => com.Id == commentId);

            if (commentToUpdate != null)
            {
                if (commentToUpdate.Likes.Contains(userId))
                {
                    commentToUpdate.Likes.Remove(userId);
                    commentToUpdate.LikesCount -= 1;
                }
                else if (!commentToUpdate.Dislikes.Contains(userId))
                {
                    commentToUpdate.Likes.Add(userId);
                    commentToUpdate.LikesCount += 1;
                }
                else if (commentToUpdate.Dislikes.Contains(userId))
                {
                    commentToUpdate.Likes.Add(userId);
                    commentToUpdate.Dislikes.Remove(userId);
                    commentToUpdate.LikesCount += 2;
                }

                await _redisRepository.SetAsync(redisKey, comments)
                    .ConfigureAwait(false);
            }

            List<User> authors = await _redisRepository.GetAsync<List<User>>(
                RedisKeys.USERS).ConfigureAwait(false);
            User userToUpdate = authors.Find(x => x.Id == userId);

            if (userToUpdate != null)
            {
                if (userToUpdate.LikedComments.Contains(commentId))
                {
                    userToUpdate.LikedComments.Remove(commentId);
                }
                else if (!userToUpdate.DislikedComments.Contains(commentId))
                {
                    userToUpdate.LikedComments.Add(commentId);
                }
                else if (userToUpdate.DislikedComments.Contains(commentId))
                {
                    userToUpdate.LikedComments.Add(commentId);
                    userToUpdate.DislikedComments.Remove(commentId);
                }

                await _redisRepository.SetAsync(RedisKeys.USERS, authors).ConfigureAwait(false);
            }

            return new Result() { Success = true };
        }

        public async Task<Result<long>> ReplyToComment(CreateReplyCommentInput input)
        {
            Comment comment = new Comment()
            {
                Content = input.Content,
                AuthorId = input.AuthorId,
                PostId = input.PostId,
                ParentCommentId = input.ParentCommentId,
                Likes = new List<long>(),
                Dislikes = new List<long>(),
                Comments = new List<long>(),
                LikesCount = 0
            };

            comment.Id = await _commentRepository
                .CreateReplyCommentAsync(comment).ConfigureAwait(false);

            List<Comment> comments = await _redisRepository.GetAsync<List<Comment>>(
                $"{input.CategorieTitle}-" + RedisKeys.COMMENTS).ConfigureAwait(false);

            Comment comment_to_update = comments.Find(x => x.Id == comment.ParentCommentId);

            if (comment_to_update != null)
            {
                comments.Add(comment);
                comment_to_update.Comments.Add(comment.Id);

                await _redisRepository.SetAsync($"{input.CategorieTitle}-" +
                    RedisKeys.COMMENTS, comments).ConfigureAwait(false);
            }

            return new Result<long>() { Success = true, Data = comment.Id };
        }
    }
}
