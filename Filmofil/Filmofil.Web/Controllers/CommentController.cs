using Microsoft.AspNetCore.Mvc;
using Filmofil.Business.CommentManagement;
using Filmofil.Business.CommentManagement.Input;
using Filmofil.Domain.Interop;
using Filmofil.Services.Repositories.Comments;
using System.Threading.Tasks;

namespace Filmofil.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    public class CommentController : ControllerBase
    {
        private ICommentManager _commentManager;
        private ICommentRepository _commentRepository;

        public CommentController(
            ICommentManager commentManager,
            ICommentRepository commentRepository)
        {
            _commentManager = commentManager;
            _commentRepository = commentRepository;
        }

        [HttpPost]
        public async Task<IActionResult> ReplyToComment([FromBody]CreateReplyCommentInput input)
        {
            Result<long> result = await _commentManager.ReplyToComment(input);
            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> LikeComment(long commentId, long userId, string categorieTitle)
        {
            Result result = await _commentManager.LikeComment(commentId, userId, categorieTitle);
            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> DislikeComment(long commentId, long userId, string categorieTitle)
        {
            Result result = await _commentManager.DislikeComment(commentId, userId, categorieTitle);
            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }
    }
}
