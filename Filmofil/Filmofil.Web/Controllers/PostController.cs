using Microsoft.AspNetCore.Mvc;
using Filmofil.Business.PostManagement;
using Filmofil.Business.PostManagement.Input;
using Filmofil.Business.ReduxLoaderManagement;
using Filmofil.Domain.Interop;
using Filmofil.Services.Repositories.Posts;
using System.Threading.Tasks;

namespace Filmofil.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    public class PostController : ControllerBase
    {
        private IPostManager _postManager;
        private IPostRepository _postRepository;
        private readonly IReduxLoaderManager _reduxLoaderManager;

        public PostController(
            IPostManager postManager,
            IPostRepository postRepository,
            IReduxLoaderManager reduxLoaderManager)
        {
            _postManager = postManager;
            _postRepository = postRepository;
            _reduxLoaderManager = reduxLoaderManager;
        }

        [HttpPost]
        public async Task<IActionResult> GetMorePosts([FromBody]MorePostsInput mpi)
        {
            Result<AppState> result;

            if (mpi.UserId == 0)
            {
                result = await _reduxLoaderManager
                    .LoadMorePosts(mpi.PostsIds, mpi.Category).ConfigureAwait(false);
            }
            else
            {
                result = await _reduxLoaderManager
                    .LoadMorePosts(mpi.PostsIds, mpi.Category, mpi.UserId).ConfigureAwait(false);
            }

            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetMorePostsOfCategorie([FromBody]MorePostsInput mpi)
        {
            Result<AppState> result = await _reduxLoaderManager.LoadMorePostsOfCategorie(
                mpi.PostsIds, mpi.Category, mpi.Categorie).ConfigureAwait(false);

            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddPost([FromBody]CreatePostInput input)
        {
            Result<long> result = await _postManager.AddPost(input);
            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddCommentToPost([FromBody]CreateCommentInput input)
        {
            Result<long> result = await _postManager.AddCommentToPost(input);
            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }

      
        

        [HttpPost]
        public async Task<IActionResult> LikePost(long postId, long userId, string categorieTitle)
        {
            Result result = await _postManager.LikePost(postId, userId, categorieTitle);
            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> DislikePost(long postId, long userId, string categorieTitle)
        {
            Result result = await _postManager.DislikePost(postId, userId, categorieTitle);
            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }
    }
}
