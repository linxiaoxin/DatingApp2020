using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helper;
using API.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IRespositories _mainRepositories;
        public LikesController(IRespositories mainRepositories)
        {
            _mainRepositories = mainRepositories;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var userId = User.GetUserId();
            var user = await _mainRepositories.UserLikesRespository.GetUserWithLikes(userId);
            var likedUser = await _mainRepositories.AppUserRespository.GetUserByUserNameAsync(username);

            if (likedUser == null)
                return NotFound();
            if (likedUser.Id == userId)
                return BadRequest("You cannot like yourself.");
            if (user.LikedUsers.Any(x => x.likedUserId == likedUser.Id))
                return BadRequest("Person was liked.");

            var userlikes = new UserLikes
            {
                likedByUserId = userId,
                likedUserId = likedUser.Id
            };
            user.LikedUsers.Add(userlikes);
            if (!await _mainRepositories.Complete())
                return BadRequest("Fail to add likes");
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikesDTO>>> GetLikes([FromQuery] LikesParams likesParmas)
        {
            likesParmas.UserId = User.GetUserId();
            var user = await _mainRepositories.UserLikesRespository.GetLikes(likesParmas);
            Response.addPaginationHeader(user.CurrentPage, user.PageSize, user.TotalCount, user.TotalPages);
            return Ok(user);
        }
    }
}