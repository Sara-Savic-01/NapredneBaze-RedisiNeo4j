using Microsoft.AspNetCore.Mvc;
using Filmofil.Business.ReduxLoaderManagement;
using Filmofil.Domain.Interop;
using System.Threading.Tasks;

namespace Filmofil.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    public class AppController : ControllerBase
    {
        private IReduxLoaderManager _reduxLoaderManager;

        public AppController(IReduxLoaderManager reduxLoaderManager)
        {
            _reduxLoaderManager = reduxLoaderManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetClientState(string category, long userId = -1)
        {
            Result<AppState> result;

            if (userId == -1)
            {
                result = await _reduxLoaderManager
                    .LoadClientState(category).ConfigureAwait(false);
            }
            else
            {
                result = await _reduxLoaderManager
                    .LoadClientState(category, userId).ConfigureAwait(false);
            }

            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }
    }
}
