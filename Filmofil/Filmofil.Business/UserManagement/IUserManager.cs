using Filmofil.Business.UserManagement.Input;
using Filmofil.Domain.Entities;
using Filmofil.Domain.InternResults;
using Filmofil.Domain.Interop;
using System.Threading.Tasks;

namespace Filmofil.Business.UserManagement
{
    public interface IUserManager
    {
        Task<Result<User>> LogIn(LogInInput input);
        Task<Result<long>> CreateUserAsync(CreateUserInput input);
        Task<Result<PostsOutput>> LoadUserPostsAsync(long userId);
        Task<Result> JoinToCategorie(JoinLeaveInput input);
        Task<Result> LeaveCategorie(JoinLeaveInput input);

        Task<Result> FollowUser(FollowUnfollowInput input);

        Task<Result> UnfollowUser(FollowUnfollowInput input);
    }
}
