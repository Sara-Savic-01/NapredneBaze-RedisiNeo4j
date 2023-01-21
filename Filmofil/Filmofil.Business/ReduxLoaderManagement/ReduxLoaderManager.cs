using Filmofil.Domain.Entities;
using Filmofil.Domain.Entities.Enums;
using Filmofil.Domain.Errors;
using Filmofil.Domain.Interop;
using Filmofil.Services.Repositories;
using Filmofil.Services.Repositories.Comments;
using Filmofil.Services.Repositories.Posts;
using Filmofil.Services.Repositories.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filmofil.Business.ReduxLoaderManagement
{
    public class ReduxLoaderManager : IReduxLoaderManager
    {
        private readonly IRedisRepository _redisRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICommentRepository _commentRepository;

        public ReduxLoaderManager(
            IRedisRepository redisRepository,
            IPostRepository postRepository,
            IUserRepository userRepository,
            ICommentRepository commentRepository)
        {
            _redisRepository = redisRepository;
            _postRepository = postRepository;
            _userRepository = userRepository;
            _commentRepository = commentRepository;
        }


        #region ImplementedMethods
        public async Task<Result<AppState>> LoadClientState(string category)
        {
            List<Categorie> categories = await _redisRepository
                .GetAsync<List<Categorie>>(RedisKeys.CATEGORIES).ConfigureAwait(false);

            if (categories == null) return new Result<AppState>() { Success = false };

            List<Post> posts = await LoadMaxAllowedPostsAsync(category).ConfigureAwait(false);

            if (posts != null && posts.Count != 0)
            {
                List<long> usersIds = posts.ConvertAll(
                    new Converter<Post, long>(UsersIdsConverter));

                List<long> postsIds = posts.ConvertAll(
                  new Converter<Post, long>(IdsConverter));

                List<long> postsCategoriesIds = posts.ConvertAll(
                    new Converter<Post, long>(CategoriesFromPostsConverter)).ToList();

                List<string> postsCategoriesTitles = categories.FindAll(categorie =>
                {
                    return postsCategoriesIds.Contains(categorie.Id) ? true : false;
                })
                .ConvertAll(new Converter<Categorie, string>(CategoriesTitleConverter)).ToList();

                List<Comment> comments = await GetCommentsAsync(postsIds, postsCategoriesTitles).ConfigureAwait(false);

                if (comments != null)
                    usersIds.AddRange(comments.ConvertAll(
                        new Converter<Comment, long>(UsersIdsConverter)).ToList());

                List<User> users = await GetUsersAsync(usersIds).ConfigureAwait(false);

                return new Result<AppState>()
                {
                    Success = true,
                    Data = new AppState()
                    {
                        Posts = posts,
                        Comments = comments,
                        Categories = categories,
                        Users = users
                    }
                };
            }

            return new Result<AppState>()
            {
                Success = true,
                Data = new AppState()
                {
                    Categories = categories
                }
            };
        }

        public async Task<Result<AppState>> LoadClientState(string category, long userId)
        {
            List<User> users = new List<User>();

            User user = await _userRepository.FindAsync(userId).ConfigureAwait(false);
            if (user == null) return new Result<AppState>() { Success = false };
            users.Add(user);

            List<Categorie> categories = await _redisRepository.GetAsync<List<Categorie>>(
                RedisKeys.CATEGORIES).ConfigureAwait(false);
            if (categories == null) return new Result<AppState>() { Success = false };

            List<string> userCategoriesTitles =
                GetCategoriesTitlesOfUser(user.Id, categories);

            List<Post> posts = await LoadMaxAllowedPostsAsync(
                category, userCategoriesTitles).ConfigureAwait(false);

            if (posts != null && posts.Count != 0)
            {
                List<long> postsIds = posts.ConvertAll(
                  new Converter<Post, long>(IdsConverter));

                List<Comment> comments = await GetCommentsAsync(
                    postsIds, userCategoriesTitles).ConfigureAwait(false);

                List<long> usersIds = posts.ConvertAll(
                    new Converter<Post, long>(UsersIdsConverter));

                usersIds.AddRange(comments.ConvertAll(
                                new Converter<Comment, long>(UsersIdsConverter)).ToList());

                users.AddRange(await GetUsersAsync(usersIds).ConfigureAwait(false));

                users = users.GroupBy(user => user.Id).Select(user2 => user2.First()).ToList();

                return new Result<AppState>()
                {
                    Success = true,
                    Data = new AppState()
                    {
                        Posts = posts,
                        Comments = comments,
                        Categories = categories,
                        Users = users
                    }
                };
            }

            return new Result<AppState>()
            {
                Data = new AppState()
                {
                    Categories = categories,
                    Users = users
                },
                Success = true
            };
        }

        public async Task<Result<AppState>> LoadMorePosts(List<long> showedPostsIds, string category)
        {
            AppState appState = new AppState();

            if (showedPostsIds != null && category != null)
            {
                List<Categorie> categories = await _redisRepository
                    .GetAsync<List<Categorie>>(RedisKeys.CATEGORIES)
                    .ConfigureAwait(false);

                appState.Posts = await LoadMoreMaxAllowedPostsAsync(
                    category, showedPostsIds).ConfigureAwait(false);

                List<long> usersIds = new List<long>();

                if (appState.Posts.Count > 0)
                {
                    List<long> postsIds = appState.Posts.ConvertAll(
                        new Converter<Post, long>(IdsConverter));

                    List<long> postsCategoriesIds = appState.Posts.ConvertAll(
                        new Converter<Post, long>(CategoriesFromPostsConverter)).ToList();

                    List<string> postsCategoriesTitles = categories.FindAll(categorie =>
                    {
                        return postsCategoriesIds.Contains(categorie.Id) ? true : false;
                    }).ConvertAll(new Converter<Categorie, string>(
                        CategoriesTitleConverter)).ToList();

                    appState.Comments = await GetCommentsAsync(
                        postsIds, postsCategoriesTitles).ConfigureAwait(false);

                    usersIds.AddRange(appState.Posts.ConvertAll(
                        new Converter<Post, long>(UsersIdsConverter)));

                    usersIds.AddRange(appState.Comments.ConvertAll(
                        new Converter<Comment, long>(UsersIdsConverter)).ToList());

                    usersIds = usersIds.Distinct().ToList();

                    appState.Users = await GetUsersAsync(usersIds).ConfigureAwait(false);
                }

                if (appState.Posts.Count >= 0 &&
                    appState.Posts.Count < RedisReduxConstraints.PostsToShow)
                {
                    int num = RedisReduxConstraints.PostsToShow - appState.Posts.Count;

                    if (showedPostsIds != null)
                        showedPostsIds.AddRange(appState.Posts.ConvertAll(
                            new Converter<Post, long>(IdsConverter)));
                    else
                        showedPostsIds = appState.Posts.ConvertAll(
                            new Converter<Post, long>(IdsConverter));

                    List<Post> postsN4J = await _postRepository
                        .FindManyPostsAsync(showedPostsIds, num, category).ConfigureAwait(false);

                    appState.Posts.AddRange(postsN4J);

                    List<long> postsIds = postsN4J.ConvertAll(
                        new Converter<Post, long>(PostsIdsConverter));

                    List<Comment> commentsN4J = await _commentRepository
                        .FindAllCommentsOfPostsAsync(postsIds).ConfigureAwait(false);

                    appState.Comments.AddRange(commentsN4J);

                    List<long> authorsIds = commentsN4J.ConvertAll(
                        new Converter<Comment, long>(AuthorIdsConverter));

                    authorsIds.AddRange(postsN4J.ConvertAll(
                        new Converter<Post, long>(UsersIdsConverter)));

                    authorsIds = authorsIds.Distinct().ToList();

                    if (usersIds.Count > 0)
                    {
                        authorsIds = authorsIds.FindAll(user => !usersIds.Contains(user));
                    }

                    appState.Users.AddRange(await _userRepository
                        .FindManyUsersAsync(authorsIds).ConfigureAwait(false));
                }

                return new Result<AppState>()
                {
                    Data = appState,
                    Success = true
                };
            }
            else
            {
                return new Result<AppState>()
                {
                    Data = appState,
                    Success = false,
                    Errors = new List<Error>()
                    {
                        new Error(ErrorCode.InvalidArguments,
                            "Method expected 2 arguments, list of showed posts and category.")
                    }
                };
            }
        }

        public async Task<Result<AppState>> LoadMorePosts(
            List<long> showedPostsIds, string category, long userId)
        {
            AppState appState = new AppState();

            if (category != null && showedPostsIds != null)
            {
                List<Categorie> categories = await _redisRepository
                    .GetAsync<List<Categorie>>(RedisKeys.CATEGORIES)
                    .ConfigureAwait(false);

                List<string> userCategoriesTitles =
                    GetCategoriesTitlesOfUser(userId, categories);

                if (userCategoriesTitles != null && userCategoriesTitles.Count > 0)
                {
                    appState.Posts = await LoadMoreMaxAllowedPostsAsync(
                        category, userCategoriesTitles, showedPostsIds)
                        .ConfigureAwait(false);

                    List<long> usersIds = new List<long>();

                    if (appState.Posts.Count > 0)
                    {
                        List<long> postsIds = appState.Posts.ConvertAll(
                        new Converter<Post, long>(IdsConverter));

                        appState.Comments = await GetCommentsAsync(
                            postsIds, userCategoriesTitles).ConfigureAwait(false);

                        usersIds.AddRange(appState.Posts.ConvertAll(
                            new Converter<Post, long>(UsersIdsConverter)));

                        usersIds.AddRange(appState.Comments.ConvertAll(
                            new Converter<Comment, long>(UsersIdsConverter)).ToList());

                        usersIds = usersIds.Distinct().ToList();

                        appState.Users = await GetUsersAsync(usersIds)
                            .ConfigureAwait(false);
                    }

                    if (appState.Posts.Count >= 0 &&
                        appState.Posts.Count < RedisReduxConstraints.PostsToShow)
                    {
                        int num = RedisReduxConstraints.PostsToShow - appState.Posts.Count;

                        if (showedPostsIds != null)
                            showedPostsIds.AddRange(appState.Posts.ConvertAll(
                                new Converter<Post, long>(IdsConverter)));
                        else
                            showedPostsIds = appState.Posts.ConvertAll(
                                new Converter<Post, long>(IdsConverter));

                        List<Post> postsN4J = await _postRepository
                            .FindMorePostsOfCategoriesAsync(showedPostsIds,
                                num, category, userCategoriesTitles)
                            .ConfigureAwait(false);

                        appState.Posts.AddRange(postsN4J);

                        List<long> postsIds = postsN4J.ConvertAll(
                            new Converter<Post, long>(PostsIdsConverter));

                        List<Comment> commentsN4J = await _commentRepository
                            .FindAllCommentsOfPostsAsync(postsIds).ConfigureAwait(false);

                        appState.Comments.AddRange(commentsN4J);

                        List<long> authorsIds = commentsN4J.ConvertAll(
                            new Converter<Comment, long>(AuthorIdsConverter));

                        authorsIds.AddRange(postsN4J.ConvertAll(
                            new Converter<Post, long>(UsersIdsConverter)));

                        authorsIds = authorsIds.Distinct().ToList();

                        if (usersIds.Count > 0)
                        {
                            authorsIds = authorsIds.FindAll(user => !usersIds.Contains(user));
                        }

                        appState.Users.AddRange(await _userRepository
                            .FindManyUsersAsync(authorsIds).ConfigureAwait(false));
                    }

                    return new Result<AppState>()
                    {
                        Data = appState,
                        Success = true
                    };
                }
                else
                {
                    return new Result<AppState>()
                    {
                        Data = appState,
                        Success = false,
                        Errors = new List<Error>() { new Error(ErrorCode.InvalidUserId) }
                    };
                }
            }
            else
            {
                return new Result<AppState>()
                {
                    Data = appState,
                    Success = false,
                    Errors = new List<Error>()
                    {
                        new Error(
                            ErrorCode.InvalidArguments,
                            "Method expected 2 arguments, list of showed posts and category.")
                    }
                };
            }
        }

        public async Task<Result<AppState>> LoadMorePostsOfCategorie(
            List<long> showedPostsIds, string category, string categorie)
        {
            AppState appState = new AppState();

            if (category != null && categorie != null && showedPostsIds != null)
            {
                List<string> categorieList = new List<string>();

                categorieList.Add(categorie);

                appState.Posts = await LoadMoreMaxAllowedPostsAsync(
                    category, categorieList, showedPostsIds).ConfigureAwait(false);

                List<long> usersIds = new List<long>();

                if (appState.Posts.Count > 0)
                {
                    List<long> postsIds = appState.Posts.ConvertAll(
                        new Converter<Post, long>(IdsConverter));

                    appState.Comments = await GetCommentsAsync(
                        postsIds, categorieList).ConfigureAwait(false);

                    usersIds.AddRange(appState.Posts.ConvertAll(
                        new Converter<Post, long>(UsersIdsConverter)));

                    usersIds.AddRange(appState.Comments.ConvertAll(
                        new Converter<Comment, long>(UsersIdsConverter)).ToList());

                    usersIds = usersIds.Distinct().ToList();

                    appState.Users = await GetUsersAsync(usersIds).ConfigureAwait(false);
                }

                if (appState.Posts.Count >= 0 &&
                    appState.Posts.Count < RedisReduxConstraints.PostsToShow)
                {
                    int num = RedisReduxConstraints.PostsToShow - appState.Posts.Count;

                    if (showedPostsIds != null)
                        showedPostsIds.AddRange(appState.Posts.ConvertAll(
                            new Converter<Post, long>(IdsConverter)));
                    else
                        showedPostsIds = appState.Posts.ConvertAll(
                            new Converter<Post, long>(IdsConverter));

                    List<Post> postsN4J = await _postRepository
                        .FindMorePostsOfCategoriesAsync(showedPostsIds,
                            num, category, categorieList)
                        .ConfigureAwait(false);

                    appState.Posts.AddRange(postsN4J);

                    List<long> postsIds = postsN4J.ConvertAll(
                        new Converter<Post, long>(PostsIdsConverter));

                    List<Comment> commentsN4J = await _commentRepository
                        .FindAllCommentsOfPostsAsync(postsIds).ConfigureAwait(false);

                    appState.Comments.AddRange(commentsN4J);

                    List<long> authorsIds = commentsN4J.ConvertAll(
                        new Converter<Comment, long>(AuthorIdsConverter));

                    authorsIds.AddRange(postsN4J.ConvertAll(
                        new Converter<Post, long>(UsersIdsConverter)));

                    authorsIds = authorsIds.Distinct().ToList();

                    if (usersIds.Count > 0)
                    {
                        authorsIds = authorsIds.FindAll(user => !usersIds.Contains(user));
                    }

                    appState.Users.AddRange(await _userRepository
                        .FindManyUsersAsync(authorsIds).ConfigureAwait(false));

                }

                return new Result<AppState>()
                {
                    Data = appState,
                    Success = true
                };
            }
            else
            {
                return new Result<AppState>()
                {
                    Data = appState,
                    Success = false,
                    Errors = new List<Error>()
                    {
                        new Error(
                            ErrorCode.InvalidArguments,
                            "Method expected 3 arguments, list of showed posts, category and categorie.")
                    }
                };
            }
        }
        #endregion

        #region Converters
        private long IdsConverter(IEntity entity)
        {
            return entity != null ? entity.Id : 0;
        }
        private long AuthorIdsConverter(Comment comment)
        {
            return comment != null ? comment.AuthorId : 0;
        }
        public static long PostsIdsConverter(Post post)
        {
            return post != null ? post.Id : 0;
        }
        public static long UsersIdsConverter(Post post)
        {
            return post != null ? post.AuthorId : 0;
        }
        public static long UsersIdsConverter(Comment cat)
        {
            return cat!= null ? cat.AuthorId : 0;
        }
        public static string CategoriesTitleConverter(Categorie cat)
        {
            return cat!= null ? cat.Title : null;
        }
        public static long CategoriesFromPostsConverter(Post post)
        {
            return post != null ? post.Categorie : 0;
        }
        #endregion

        #region AdditionalMethods

        private async Task<List<Post>> LoadMaxAllowedPostsAsync(string category)
        {
            List<Post> posts = await _redisRepository
                .GetAsync<List<Post>>(category).ConfigureAwait(false);

            if (posts == null) return null;

            int range = posts.Count > RedisReduxConstraints.PostsToShow ?
                RedisReduxConstraints.PostsToShow : posts.Count;

            return posts.GetRange(0, range);
        }

        private async Task<List<Post>> LoadMaxAllowedPostsAsync(
            string category, List<string> CategoriesTitles)
        {
            List<Post> posts = new List<Post>();

            foreach (string title in CategoriesTitles)
            {
                string key = title + "-" + category;
                List<Post> postsFromReddis = await _redisRepository
                    .GetAsync<List<Post>>(key).ConfigureAwait(false);

                if (postsFromReddis != null)
                    posts.AddRange(postsFromReddis);
            }

            return PreparePostsForReturning(posts, category);
        }

        private async Task<List<Post>> LoadMoreMaxAllowedPostsAsync(
            string category, List<long> showedPostsIds)
        {
            List<Post> posts = new List<Post>();

            posts = await _redisRepository.GetAsync<List<Post>>(category)
                .ConfigureAwait(false);

            if (posts == null) return new List<Post>();

            posts = posts.FindAll(post =>
            {
                return showedPostsIds.Contains(post.Id) ? false : true;
            });

            int range = posts.Count > RedisReduxConstraints.PostsToShow ?
                RedisReduxConstraints.PostsToShow : posts.Count;

            return posts.GetRange(0, range);
        }

        private async Task<List<Post>> LoadMoreMaxAllowedPostsAsync(
            string category, List<string> CategoriesTitles, List<long> showedPostsIds)
        {
            List<Post> posts = new List<Post>();

            foreach (string title in CategoriesTitles)
            {
                string key = title + "-" + category;
                List<Post> postsToAdd = await _redisRepository
                    .GetAsync<List<Post>>(key).ConfigureAwait(false);

                if (postsToAdd != null)
                {
                    postsToAdd = postsToAdd.FindAll(post =>
                    {
                        return showedPostsIds.Contains(post.Id) ? false : true;
                    });

                    int range = postsToAdd.Count > RedisReduxConstraints.PostsToShow ?
                    RedisReduxConstraints.PostsToShow : postsToAdd.Count;

                    posts.AddRange(postsToAdd.GetRange(0, range));
                }
            }

            return PreparePostsForReturning(posts, category);
        }

        private List<Post> PreparePostsForReturning(List<Post> posts, string category)
        {
            if (category == RedisKeys.NEW_POSTS)
                posts.Sort((x, y) => -x.TimeStamp.CompareTo(y.TimeStamp));
            else if (category == RedisKeys.BEST_POSTS)
                posts.Sort((x, y) => -x.LikesCount.CompareTo(y.LikesCount));
            else if (category == RedisKeys.POPULAR_POSTS)
                posts.Sort((x, y) => -x.Popularity.CompareTo(y.Popularity));

            int range = posts.Count > RedisReduxConstraints.PostsToShow ?
                RedisReduxConstraints.PostsToShow : posts.Count;

            return posts.GetRange(0, range);
        }

        private async Task<List<Comment>> GetCommentsAsync(
            List<long> postsIds, List<string> userCategoriesTitles)
        {
            List<Comment> comments = new List<Comment>();

            foreach (string title in userCategoriesTitles)
            {
                string key = title + "-" + RedisKeys.COMMENTS;

                List<Comment> commentsFromRedis = await _redisRepository
                    .GetAsync<List<Comment>>(key).ConfigureAwait(false);

                if (commentsFromRedis != null)
                    comments.AddRange(commentsFromRedis);
            }

            return comments.FindAll(comment =>
            {
                return postsIds.Contains(comment.PostId) ? true : false;
            });
        }

        private async Task<List<User>> GetUsersAsync(List<long> ids)
        {
            List<User> users = await _redisRepository.GetAsync<List<User>>
                (RedisKeys.USERS).ConfigureAwait(false);

            if (users != null)
                return users.FindAll(user =>
                {
                    return ids.Contains(user.Id) ? true : false;
                });
            else
                return new List<User>();
        }

        private List<string> GetCategoriesTitlesOfUser(
            long userId, List<Categorie> allCategories)
        {
            List<string> userCategoriesTitles = new List<string>();

            foreach (Categorie x in allCategories)
            {
                if (x.Users.Contains(userId))
                    userCategoriesTitles.Add(x.Title);
            }

            return userCategoriesTitles;
        }
        #endregion
    }
}
