using Filmofil.Business.UserManagement.Input;
using Filmofil.Domain.Entities;
using Filmofil.Domain.Entities.Enums;
using Filmofil.Domain.InternResults;
using Filmofil.Domain.Interop;
using Filmofil.Services.Repositories;
using Filmofil.Services.Repositories.Comments;
using Filmofil.Services.Repositories.Categories;
using Filmofil.Services.Repositories.Posts;
using Filmofil.Services.Repositories.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filmofil.Business.UserManagement
{
    public class UserManager : IUserManager
    {

        private IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;
        private readonly IRedisRepository _redisRepository;
        private readonly ICategorieRepository _categorieRepository;
        private readonly ICommentRepository _commentRepository;

        public UserManager(IUserRepository userRepository,
                            IPostRepository postRepository,
                            IRedisRepository redisRepository,
                            ICategorieRepository categorieRepository,
                            ICommentRepository commentRepository)
        {
            _userRepository = userRepository;
            _postRepository = postRepository;
            _redisRepository = redisRepository;
            _categorieRepository = categorieRepository;
            _commentRepository = commentRepository;
        }

        public async Task<Result<long>> CreateUserAsync(CreateUserInput input)
        {
            User user = new User()
            {
                Email = input.Email,
                Username = input.Username,
                Password = input.Password,
                Posts = new List<long>(),
                Comments = new List<long>(),
                LikedPosts = new List<long>(),
                LikedComments = new List<long>(),
                DislikedPosts = new List<long>(),
                DislikedComments = new List<long>(),
                Categories = new List<long>(),
                Followers = new List<long>(),
                Following = new List<long>()
            };

            if (!(await _userRepository.CheckUserByUsernameAsync(user.Username, user.Password)))
            {
                return new Result<long>()
                {
                    Success = true,
                    Data = await _userRepository.CreateAsync(user)
                };
            }

            return new Result<long>() { Success = false };
        }

        public async Task<Result<User>> LogIn(LogInInput input)
        {
            User user = await _userRepository.GetUserByUsernameAsync(
                input.Username, input.Password).ConfigureAwait(false);

            if (user == null) return new Result<User>() { Success = false };

            return new Result<User>() { Success = true, Data = user };
        }

        public async Task<Result<PostsOutput>> LoadUserPostsAsync(long userId)
        {
            PostsOutput posts = new PostsOutput();

            posts.Posts = await _postRepository.GetPostsOfUserAsync(userId);

            List<long> postsIds = posts.Posts.ConvertAll(new Converter<Post, long>(PostsIdsConverter));

            posts.PostsComments = await _commentRepository.FindAllCommentsOfPostsAsync(postsIds).ConfigureAwait(false);

            List<long> authorsIds = posts.PostsComments.ConvertAll(new Converter<Comment, long>(AuthorIdsConverter));
            authorsIds = authorsIds.Distinct().ToList();

            posts.Authors = await _userRepository.FindManyUsersAsync(authorsIds).ConfigureAwait(false);

            if (posts != null)
            {
                return new Result<PostsOutput>()
                {
                    Data = posts,
                    Success = true
                };
            }
            else
            {
                return new Result<PostsOutput>()
                {
                    Data = new PostsOutput(),
                    Success = false
                };
            }
        }
        public async Task<Result> FollowUser(FollowUnfollowInput input)
        {
            await _userRepository.FollowUserAsync(
                input.FollowerId, input.FollowingId).ConfigureAwait(false);

            List<User> users = await _redisRepository
                .GetAsync<List<User>>(RedisKeys.USERS).ConfigureAwait(false);

            User followUser = users.Find(user => user.Id == input.FollowerId);
            User followingUser = users.Find(user => user.Id == input.FollowingId);

            if(followUser != null)
            {
                followUser.Following.Add(followingUser.Id);
                await _redisRepository.SetAsync(
                     RedisKeys.USERS, users).ConfigureAwait(false);
            }
            Console.WriteLine(followUser.Following);

            if(followingUser != null)
            {
                followingUser.Followers.Add(followUser.Id);
                await _redisRepository.SetAsync(
                    RedisKeys.USERS, users).ConfigureAwait(false);
            }
            Console.WriteLine(followingUser.Followers);

            return new Result() { Success = true };
        }

        public async Task<Result> UnfollowUser(FollowUnfollowInput input)
        {
            await _userRepository.UnfollowUserAsync(
                input.FollowerId, input.FollowingId).ConfigureAwait(false);

            List<User> users = await _redisRepository
                .GetAsync<List<User>>(RedisKeys.USERS).ConfigureAwait(false);

            User followUser = users.Find(user => user.Id == input.FollowerId);
            User followingUser = users.Find(user => user.Id == input.FollowingId);

            if (followUser != null)
            {
                followUser.Following.Remove(followingUser.Id);
                await _redisRepository.SetAsync(
                    RedisKeys.USERS, users).ConfigureAwait(false);
            }


            if (followingUser != null)
            {
                followingUser.Followers.Remove(followUser.Id);
                await _redisRepository.SetAsync(
                    RedisKeys.USERS, users).ConfigureAwait(false);
            }

            return new Result() { Success = true };

        }

        public async Task<Result> JoinToCategorie(JoinLeaveInput input)
        {
            await _userRepository.JoinToCategorieAsync(
                input.CategorieId, input.UserId).ConfigureAwait(false);

            List<Categorie> categories = await _redisRepository
                .GetAsync<List<Categorie>>(RedisKeys.CATEGORIES).ConfigureAwait(false);

            Categorie categorie = categories.Find(cat=> cat.Id == input.CategorieId);

            List<User> authors = await _redisRepository
                .GetAsync<List<User>>(RedisKeys.USERS).ConfigureAwait(false);

            User user = authors.Find(user => user.Id == input.UserId);

            if (categorie != null)
            {
                categorie.Users.Add(input.UserId);
                await _redisRepository.SetAsync(
                    RedisKeys.CATEGORIES, categories).ConfigureAwait(false);
            }

            if (user != null)
            {
                user.Categories.Add(input.CategorieId);

                await _redisRepository.SetAsync(RedisKeys.USERS, authors)
                    .ConfigureAwait(false);
            }

            return new Result() { Success = true };
        }

        public async Task<Result> LeaveCategorie(JoinLeaveInput input)
        {
            await _userRepository.LeaveCategorieAsync(
                input.CategorieId, input.UserId).ConfigureAwait(false);

            List<Categorie> categories = await _redisRepository
                .GetAsync<List<Categorie>>(RedisKeys.CATEGORIES).ConfigureAwait(false);

            Categorie categorie = categories.Find(cat=> cat.Id == input.CategorieId);

            List<User> authors = await _redisRepository
                .GetAsync<List<User>>(RedisKeys.USERS).ConfigureAwait(false);

            User user = authors.Find(user => user.Id == input.UserId);

            if (categorie != null)
            {
                categorie.Users.Remove(input.UserId);

                await _redisRepository.SetAsync(
                    RedisKeys.CATEGORIES, categories).ConfigureAwait(false);
            }

            if (user != null)
            {
                user.Categories.Remove(input.CategorieId);

                await _redisRepository.SetAsync(
                    RedisKeys.USERS, authors).ConfigureAwait(false);
            }

            return new Result() { Success = true };
        }

        private long PostsIdsConverter(Post post)
        {
            return post != null ? post.Id : 0;
        }
        private long AuthorIdsConverter(Comment comment)
        {
            return comment != null ? comment.AuthorId : 0;
        }
        public static long UsersIdsConverter(Post post)
        {
            return post != null ? post.AuthorId : 0;
        }
    }
}
