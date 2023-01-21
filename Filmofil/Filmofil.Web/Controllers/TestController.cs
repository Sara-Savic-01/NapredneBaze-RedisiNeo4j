using Microsoft.AspNetCore.Mvc;
using Filmofil.Services.Repositories.Users;
using System.Threading.Tasks;

namespace Filmofil.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    public class TestController : Controller
    {
        private readonly IUserRepository userRepository;

        public TestController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult> Test()
        {
            userRepository.Delete(123);
            return Ok();
        }
    }
}
