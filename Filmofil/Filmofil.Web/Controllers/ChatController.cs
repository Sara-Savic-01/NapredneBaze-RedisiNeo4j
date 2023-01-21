using Microsoft.AspNetCore.Mvc;
using Filmofil.Business.ChatManagement;
using Filmofil.Domain.Entities;
using Filmofil.Domain.Interop;
using System.Threading.Tasks;

namespace Filmofil.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    public class ChatController : ControllerBase
    {
        private readonly IChatManager _chatManager;

        public ChatController(IChatManager chatManager)
        {
            _chatManager = chatManager;
        }

        [HttpPost]
        public async Task<IActionResult> LoadChatDataAndSubscribe([FromBody]SubscribeInput input)
        {
            Result<LoadChatDataOutput> result = await _chatManager
                .LoadChatDataAndSubscribe(input).ConfigureAwait(false);
            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody]Message message)
        {
            Result<long> result = await _chatManager.SendMessage(message)
                .ConfigureAwait(false);
            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }
    }
}
