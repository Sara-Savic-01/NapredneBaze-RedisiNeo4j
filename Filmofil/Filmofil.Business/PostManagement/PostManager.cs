using Filmofil.Business.PostManagement.Input;
using Filmofil.Business.ReduxLoaderManagement;
using Filmofil.Domain.Entities;
using Filmofil.Domain.Entities.Enums;
using Filmofil.Domain.Interop;
using Filmofil.Services.Repositories;
using Filmofil.Services.Repositories.Comments;
using Filmofil.Services.Repositories.Posts;
using Filmofil.Services.Repositories.Users;
using Filmofil.Services.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Filmofil.Business.PostManagement
{
    public class PostManager : IPostManager
    {
        private IPostRepository _postRepository;
        private ICommentRepository _commentRepository;
        private IUserRepository _userRepository;
        private IRedisRepository _redisRepository;
        private IPostService _postService;

        public PostManager(IPostRepository postRepository,
            IUserRepository userRepository,
            IRedisRepository redisRepository,
            ICommentRepository commentRepository,
            IPostService postService)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _userRepository = userRepository;
            _redisRepository = redisRepository;
            _postService = postService;
        }

        #region Implemented methods
        public async Task<Result<long>> AddCommentToPost(CreateCommentInput input)
        {
            if (input == null) return new Result<long>() { Success = false };
            Comment comment = new Comment()
            {
                Content = input.Content,
                AuthorId = input.AuthorId,
                PostId = input.PostId,
                Likes = new List<long>(),
                Dislikes = new List<long>(),
                Comments = new List<long>(),
                LikesCount = 0
            };
            comment.Id = await _commentRepository
                .CreateAsync(comment).ConfigureAwait(false);

            Post post = await _postRepository
                .FindAsync(comment.PostId).ConfigureAwait(false);

            post.Popularity = _postService.PostPopularity(post);

            await SetUserInReddisComm(comment.AuthorId, comment.Id)
                .ConfigureAwait(false);

            string categorieName = post.CategorieTitle;

            List<Comment> comments = await _redisRepository.GetAsync<List<Comment>>(
                $"{categorieName}-" + RedisKeys.COMMENTS).ConfigureAwait(false);

            SetCommentInReddis(post, comment, comments, RedisKeys.POPULAR_POSTS);
            SetCommentInReddis(post, comment, comments, RedisKeys.NEW_POSTS);
            SetCommentInReddis(post, comment, comments, RedisKeys.BEST_POSTS);

            return new Result<long>() { Success = true, Data = comment.Id };
        }

        public async Task<Result<long>> AddPost(CreatePostInput input)
        {
            List<Categorie> categories = await _redisRepository
                .GetAsync<List<Categorie>>(RedisKeys.CATEGORIES).ConfigureAwait(false);

            Categorie categorie = categories.Find(x => x.Title == input.CategorieTitle);

            Post post = new Post()
            {
                Content = input.Content,
                Categorie = categorie.Id,
                CategorieTitle = input.CategorieTitle,
                AuthorId = input.AuthorId,
                LikesCount = 0,
                Likes = new List<long>(),
                Dislikes = new List<long>(),
                Comments = new List<long>(),
                TimeStamp = DateTime.Now.ToOADate() - 43800.00,
                Popularity = 0.0
            };

            post.Id = await _postRepository.CreateAsync(post).ConfigureAwait(false);

            categorie.Posts.Add(post.Id);
            await _redisRepository.SetAsync("categories", categories).ConfigureAwait(false);

            await SetUserInReddisPost(post.AuthorId, post.Id).ConfigureAwait(false);
            string categorieName = post.CategorieTitle;

            List<Post> categorieName_new_posts = await _redisRepository
                .GetAsync<List<Post>>($"{categorieName}-" + RedisKeys.NEW_POSTS).ConfigureAwait(false);

            List<Post> new_posts = await _redisRepository
                .GetAsync<List<Post>>(RedisKeys.NEW_POSTS).ConfigureAwait(false);

            if (new_posts.Count < RedisReduxConstraints.MaxAllowedPostsInRedis)
                new_posts.Insert(0, post);
            else
            {
                new_posts.RemoveAt(RedisReduxConstraints.MaxAllowedPostsInRedis - 1);
                new_posts.Insert(0, post);
            }

            if (categorieName_new_posts.Count < RedisReduxConstraints.MaxAllowedPostsInRedis)
                categorieName_new_posts.Insert(0, post);
            else
            {
                await RemoveCommentFromRedisAsync(categorieName_new_posts
                    [RedisReduxConstraints.MaxAllowedPostsInRedis - 1]).ConfigureAwait(false);

                categorieName_new_posts.RemoveAt(
                    RedisReduxConstraints.MaxAllowedPostsInRedis - 1);

                categorieName_new_posts.Insert(0, post);
            }

            await _redisRepository.SetAsync(RedisKeys.NEW_POSTS, new_posts)
                .ConfigureAwait(false);

            await _redisRepository.SetAsync($"{categorieName}-" + RedisKeys.NEW_POSTS,
                categorieName_new_posts).ConfigureAwait(false);

            return new Result<long>() { Success = true, Data = post.Id };
        }

        public async Task<Result> DislikePost(long postId, long userId, string categorieTitle)
        {
            await _postRepository.DislikePostAsync(postId, userId).ConfigureAwait(false);

            List<User> authors = await _redisRepository
                .GetAsync<List<User>>(RedisKeys.USERS).ConfigureAwait(false);

            User userToUpdate = authors.Find(x => x.Id == userId);

            if (userToUpdate != null)
            {
                if (userToUpdate.DislikedPosts.Contains(postId))
                {
                    userToUpdate.DislikedPosts.Remove(postId);
                }
                else if (!userToUpdate.LikedPosts.Contains(postId))
                {
                    userToUpdate.DislikedPosts.Add(postId);
                }
                else if (userToUpdate.LikedPosts.Contains(postId))
                {
                    userToUpdate.DislikedPosts.Add(postId);
                    userToUpdate.LikedPosts.Remove(postId);
                }

                await _redisRepository.SetAsync(RedisKeys.USERS, authors)
                    .ConfigureAwait(false);
            }

            SetDislikesInReddis(postId, userId, categorieTitle, RedisKeys.POPULAR_POSTS);
            SetDislikesInReddis(postId, userId, categorieTitle, RedisKeys.NEW_POSTS);
            SetDislikesInReddis(postId, userId, categorieTitle, RedisKeys.BEST_POSTS);

            return new Result() { Success = true };
        }

        public async Task<Result> LikePost(long postId, long userId, string categorieTitle)
        {
            await _postRepository.LikePostAsync(postId, userId)
                .ConfigureAwait(false);

            List<User> authors = await _redisRepository
                .GetAsync<List<User>>(RedisKeys.USERS).ConfigureAwait(false);

            User userToUpdate = authors.Find(x => x.Id == userId);

            if (userToUpdate != null)
            {
                if (userToUpdate.LikedPosts.Contains(postId))
                {
                    userToUpdate.LikedPosts.Remove(postId);
                }
                else if (!userToUpdate.DislikedPosts.Contains(postId))
                {
                    userToUpdate.LikedPosts.Add(postId);
                }
                else if (userToUpdate.DislikedPosts.Contains(postId))
                {
                    userToUpdate.LikedPosts.Add(postId);
                    userToUpdate.DislikedPosts.Remove(postId);
                }

                await _redisRepository.SetAsync(RedisKeys.USERS, authors)
                    .ConfigureAwait(false);
            }

            SetLikesInReddis(postId, userId, categorieTitle, RedisKeys.POPULAR_POSTS);
            SetLikesInReddis(postId, userId, categorieTitle, RedisKeys.NEW_POSTS);
            SetLikesInReddis(postId, userId, categorieTitle, RedisKeys.BEST_POSTS);

            return new Result() { Success = true };
        }
        #endregion

        #region Additonal functions
        private async Task RemoveCommentFromRedisAsync(Post post)
        {
            string key = post.CategorieTitle + "-" + RedisKeys.COMMENTS;

            List<Comment> categorieComments = await
                _redisRepository.GetAsync<List<Comment>>(key).ConfigureAwait(false);

            if (categorieComments.RemoveAll(comment => comment.PostId == post.Id) > 0)
                await _redisRepository.SetAsync(key, categorieComments)
                    .ConfigureAwait(false);
        }

        private async void SetCommentInReddis(Post postToCheck, Comment commentToSet, List<Comment> commentsToUpdate, string type)
        {
            List<Post> categorie_posts = await _redisRepository.GetAsync<List<Post>>(
                $"{postToCheck.CategorieTitle}-" + type).ConfigureAwait(false);

            List<Post> type_posts = await _redisRepository
                .GetAsync<List<Post>>(type).ConfigureAwait(false);

            Post comunityPostToUpDate = categorie_posts.Find(post => post.Id == postToCheck.Id);
            Post typePostToUpDate = type_posts.Find(post => post.Id == postToCheck.Id);

            if (comunityPostToUpDate != null)
            {
                if (!commentsToUpdate.Contains(commentToSet))
                {
                    commentsToUpdate.Add(commentToSet);

                    await _redisRepository.SetAsync($"{postToCheck.CategorieTitle}-" +
                        RedisKeys.COMMENTS, commentsToUpdate).ConfigureAwait(false);
                }

                comunityPostToUpDate.Popularity = postToCheck.Popularity;
                comunityPostToUpDate.Comments.Add(commentToSet.Id);

                if (type == RedisKeys.POPULAR_POSTS)
                    categorie_posts.Sort((x, y) => -x.Popularity.CompareTo(y.Popularity));

                await _redisRepository.SetAsync($"{postToCheck.CategorieTitle}-" + type,
                    categorie_posts).ConfigureAwait(false);
            }
            else if (type == RedisKeys.POPULAR_POSTS)
                await CheckCategoriePosts(postToCheck, categorie_posts, type,
                    $"{postToCheck.CategorieTitle}-" + type).ConfigureAwait(false);

            if (typePostToUpDate != null)
            {
                typePostToUpDate.Popularity = postToCheck.Popularity;
                typePostToUpDate.Comments.Add(commentToSet.Id);

                if (type == RedisKeys.POPULAR_POSTS)
                    type_posts.Sort((x, y) => -x.Popularity.CompareTo(y.Popularity));

                await _redisRepository.SetAsync(type, type_posts).ConfigureAwait(false);
            }
            else if (type == RedisKeys.POPULAR_POSTS)
                await CheckCategoriePosts(postToCheck, type_posts, type, type)
                    .ConfigureAwait(false);
        }

        private async void SetLikesInReddis(long postId, long userId, string categorieTitle, string type)
        {
            string redisKey = categorieTitle + "-" + type;

            List<Post> categorie_posts = await _redisRepository
                .GetAsync<List<Post>>(redisKey).ConfigureAwait(false);

            List<Post> type_posts = await _redisRepository
                .GetAsync<List<Post>>(type).ConfigureAwait(false);

            Post comunityPostToUpDate = categorie_posts.Find(post => (post.Id == postId));
            Post typePostToUpDate = type_posts.Find(post => (post.Id == postId));
            Post postN4J = null;

            if (comunityPostToUpDate != null)
            {
                if (comunityPostToUpDate.Likes.Contains(userId))
                {
                    comunityPostToUpDate.Likes.Remove(userId);
                    comunityPostToUpDate.LikesCount -= 1;
                }
                else if (!comunityPostToUpDate.Dislikes.Contains(userId))
                {
                    comunityPostToUpDate.Likes.Add(userId);
                    comunityPostToUpDate.LikesCount += 1;
                }
                else if (comunityPostToUpDate.Dislikes.Contains(userId))
                {
                    comunityPostToUpDate.Likes.Add(userId);
                    comunityPostToUpDate.Dislikes.Remove(userId);
                    comunityPostToUpDate.LikesCount += 2;
                }

                comunityPostToUpDate.Popularity = _postService
                    .PostPopularity(comunityPostToUpDate);

                if (type == RedisKeys.POPULAR_POSTS)
                    categorie_posts.Sort((x, y) => -x.Popularity.CompareTo(y.Popularity));
                else if (type == RedisKeys.BEST_POSTS)
                {
                    if (comunityPostToUpDate.LikesCount <= 0)
                        categorie_posts.Remove(comunityPostToUpDate);
                    else
                        categorie_posts.Sort((x, y) => -x.LikesCount.CompareTo(y.LikesCount));
                }

                await _redisRepository.SetAsync(redisKey, categorie_posts).ConfigureAwait(false);
            }
            else if (!(type == RedisKeys.NEW_POSTS))
            {
                postN4J = await _postRepository.FindAsync(postId).ConfigureAwait(false);

                await CheckCategoriePosts(postN4J, categorie_posts, type, redisKey)
                    .ConfigureAwait(false);
            }

            if (typePostToUpDate != null)
            {
                if (typePostToUpDate.Likes.Contains(userId))
                {
                    typePostToUpDate.Likes.Remove(userId);
                    typePostToUpDate.LikesCount -= 1;
                }
                else if (!typePostToUpDate.Dislikes.Contains(userId))
                {
                    typePostToUpDate.Likes.Add(userId);
                    typePostToUpDate.LikesCount += 1;
                }
                else if (typePostToUpDate.Dislikes.Contains(userId))
                {
                    typePostToUpDate.Likes.Add(userId);
                    typePostToUpDate.Dislikes.Remove(userId);
                    typePostToUpDate.LikesCount += 2;
                }

                typePostToUpDate.Popularity = _postService.PostPopularity(typePostToUpDate);

                if (type == RedisKeys.POPULAR_POSTS)
                    type_posts.Sort((x, y) => -x.Popularity.CompareTo(y.Popularity));
                else if (type == RedisKeys.BEST_POSTS)
                {
                    if (typePostToUpDate.LikesCount <= 0)
                        type_posts.Remove(typePostToUpDate);
                    else
                        type_posts.Sort((x, y) => -x.LikesCount.CompareTo(y.LikesCount));
                }

                await _redisRepository.SetAsync(type, type_posts).ConfigureAwait(false);
            }
            else if (!(type == RedisKeys.NEW_POSTS))
            {
                if (postN4J == null)
                    postN4J = await _postRepository.FindAsync(postId).ConfigureAwait(false);
                await CheckCategoriePosts(postN4J, type_posts, type, type).ConfigureAwait(false);
            }
        }

        private async void SetDislikesInReddis(long postId, long userId, string categorieTitle, string type)
        {
            string redisKey = categorieTitle + "-" + type;

            List<Post> categorie_posts = await _redisRepository
                .GetAsync<List<Post>>(redisKey).ConfigureAwait(false);

            List<Post> type_posts = await _redisRepository
                .GetAsync<List<Post>>(type).ConfigureAwait(false);

            Post comunityPostToUpDate = categorie_posts.Find(post => (post.Id == postId));
            Post typePostToUpDate = type_posts.Find(post => (post.Id == postId));
            Post postN4J = null;

            if (comunityPostToUpDate != null)
            {
                if (comunityPostToUpDate.Dislikes.Contains(userId))
                {
                    comunityPostToUpDate.Dislikes.Remove(userId);
                    comunityPostToUpDate.LikesCount += 1;
                }
                else if (!comunityPostToUpDate.Likes.Contains(userId))
                {
                    comunityPostToUpDate.Dislikes.Add(userId);
                    comunityPostToUpDate.LikesCount -= 1;
                }
                else if (comunityPostToUpDate.Likes.Contains(userId))
                {
                    comunityPostToUpDate.Dislikes.Add(userId);
                    comunityPostToUpDate.Likes.Remove(userId);
                    comunityPostToUpDate.LikesCount -= 2;
                }
                comunityPostToUpDate.Popularity = _postService
                    .PostPopularity(comunityPostToUpDate);

                if (type == RedisKeys.POPULAR_POSTS)
                    categorie_posts.Sort((x, y) => -x.Popularity.CompareTo(y.Popularity));
                else if (type == RedisKeys.BEST_POSTS)
                {
                    if (comunityPostToUpDate.LikesCount <= 0)
                        categorie_posts.Remove(comunityPostToUpDate);
                    else
                        categorie_posts.Sort((x, y) => -x.LikesCount.CompareTo(y.LikesCount));
                }

                await _redisRepository.SetAsync(redisKey, categorie_posts).ConfigureAwait(false);
            }
            else if (!(type == RedisKeys.NEW_POSTS))
            {
                postN4J = await _postRepository.FindAsync(postId).ConfigureAwait(false);

                await CheckCategoriePosts(postN4J, categorie_posts, type, redisKey)
                    .ConfigureAwait(false);
            }

            if (typePostToUpDate != null)
            {
                if (typePostToUpDate.Dislikes.Contains(userId))
                {
                    typePostToUpDate.Dislikes.Remove(userId);
                    typePostToUpDate.LikesCount += 1;
                }
                else if (!typePostToUpDate.Likes.Contains(userId))
                {
                    typePostToUpDate.Dislikes.Add(userId);
                    typePostToUpDate.LikesCount -= 1;
                }
                else if (typePostToUpDate.Likes.Contains(userId))
                {
                    typePostToUpDate.Dislikes.Add(userId);
                    typePostToUpDate.Likes.Remove(userId);
                    typePostToUpDate.LikesCount -= 2;
                }
                typePostToUpDate.Popularity = _postService
                    .PostPopularity(typePostToUpDate);

                if (type == RedisKeys.POPULAR_POSTS)
                    type_posts.Sort((x, y) => -x.Popularity.CompareTo(y.Popularity));
                else if (type == RedisKeys.BEST_POSTS)
                {
                    if (typePostToUpDate.LikesCount <= 0)
                        type_posts.Remove(typePostToUpDate);
                    else
                        type_posts.Sort((x, y) => -x.LikesCount.CompareTo(y.LikesCount));
                }

                await _redisRepository.SetAsync(type, type_posts).ConfigureAwait(false);
            }
            else if (!(type == RedisKeys.NEW_POSTS))
            {
                if (postN4J == null)
                    postN4J = await _postRepository.FindAsync(postId).ConfigureAwait(false);

                await CheckCategoriePosts(postN4J, type_posts, type, type)
                    .ConfigureAwait(false);
            }
        }

        private async Task SetUserInReddisPost(long userId, long postId)
        {
            List<User> users = await _redisRepository
                .GetAsync<List<User>>(RedisKeys.USERS).ConfigureAwait(false);

            if (users == null) users = new List<User>();

            User userToUpDate = users.Find(userToCheck => userToCheck.Id == userId);

            if (userToUpDate != null)
            {
                userToUpDate.Posts.Add(postId);
            }
            else
            {
                User user = await _userRepository
                    .FindAsync(userId).ConfigureAwait(false);

                users.Add(user);
            }

            await _redisRepository.SetAsync(RedisKeys.USERS, users)
                .ConfigureAwait(false);
        }

        private async Task SetUserInReddisComm(long userId, long commId)
        {
            List<User> users = await _redisRepository
                .GetAsync<List<User>>(RedisKeys.USERS).ConfigureAwait(false);

            if (users == null) users = new List<User>();

            User userToUpDate = users.Find(userToCheck => userToCheck.Id == userId);

            if (userToUpDate != null)
            {
                userToUpDate.Comments.Add(commId);
            }
            else
            {
                User user = await _userRepository.FindAsync(userId)
                    .ConfigureAwait(false);

                users.Add(user);
            }

            await _redisRepository.SetAsync(RedisKeys.USERS, users)
                .ConfigureAwait(false);
        }

        private async Task CheckCategoriePosts(Post post,
            List<Post> postsList, string type, string key)
        {
            bool toAdd = false;
            if (type == RedisKeys.BEST_POSTS &&
                postsList.Count < RedisReduxConstraints.MaxAllowedPostsInRedis &&
                post.LikesCount > 0)
            {
                toAdd = true;
            }
            else if (type == RedisKeys.POPULAR_POSTS &&
                postsList.Count < RedisReduxConstraints.MaxAllowedPostsInRedis)
            {
                toAdd = true;
            }
            else if (type == RedisKeys.BEST_POSTS &&
                postsList.Count == RedisReduxConstraints.MaxAllowedPostsInRedis &&
                postsList[RedisReduxConstraints.MaxAllowedPostsInRedis - 1].LikesCount < post.LikesCount)
            {
                postsList.RemoveAt(RedisReduxConstraints.MaxAllowedPostsInRedis - 1);
                toAdd = true;
            }
            else if (type == RedisKeys.POPULAR_POSTS &&
                postsList.Count == RedisReduxConstraints.MaxAllowedPostsInRedis &&
                postsList[RedisReduxConstraints.MaxAllowedPostsInRedis - 1].Popularity < post.Popularity)
            {
                postsList.RemoveAt(RedisReduxConstraints.MaxAllowedPostsInRedis - 1);
                toAdd = true;
            }

            if (toAdd)
            {
                postsList.Add(post);
                await _redisRepository.SetAsync(key, postsList).ConfigureAwait(false);
            }
        }
        #endregion
    }
}
