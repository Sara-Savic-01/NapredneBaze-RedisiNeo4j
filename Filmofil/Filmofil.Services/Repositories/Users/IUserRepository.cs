using Filmofil.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Filmofil.Services.Repositories.Users
{
    public interface IUserRepository : INeo4JRepository<User>
    {
        Task<User> GetUserByUsernameAsync(string username, string password);
        Task<bool> CheckUserByUsernameAsync(string username, string password);
        Task FollowUserAsync(long followerId, long followingId);
        Task UnfollowUserAsync(long followerId, long followingId);
        Task JoinToCategorieAsync(long categorieId, long userId);
        Task LeaveCategorieAsync(long categorieId, long userId);
        Task<List<User>> FindManyUsersAsync(List<long> userIds);
    }
}
