using Microsoft.AspNetCore.Mvc;
using Filmofil.Business.UserManagement;
using Filmofil.Business.UserManagement.Input;
using Filmofil.Domain.Entities;
using Filmofil.Domain.InternResults;
using Filmofil.Domain.Interop;
using System.Threading.Tasks;

namespace Filmofil.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private IUserManager _userManager;

        public UserController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserPosts(long userId)
        {
            Result<PostsOutput> result = await _userManager.LoadUserPostsAsync(userId);
            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody]CreateUserInput input)
        {
            Result<long> result = await _userManager.CreateUserAsync(input);
            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> LogIn([FromBody]LogInInput input)
        {
            Result<User> result = await _userManager.LogIn(input);
            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> FollowUser([FromBody] FollowUnfollowInput input)
        {
            Result result = await _userManager.FollowUser(input);
            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> UnfollowUser([FromBody] FollowUnfollowInput input)
        {
            Result result = await _userManager.UnfollowUser(input);
            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> JoinToCategorie([FromBody]JoinLeaveInput input)
        {
            Result result = await _userManager.JoinToCategorie(input);
            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> LeaveCategorie([FromBody]JoinLeaveInput input)
        {
            Result result = await _userManager.LeaveCategorie(input);
            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }
    }
}
