using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helper;
using API.Interface;
using AutoMapper;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly IRespositories _mainRespositories;
        public UsersController(IRespositories mainRespositories, IMapper mapper, IPhotoService photoService)
        {
            _mainRespositories = mainRespositories;
            _photoService = photoService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers([FromQuery] UserParams userParams)
        {
            userParams.UserName = User.GetUserName();
            var user = await _mainRespositories.AppUserRespository.GetMembersAsync(userParams);
            Response.addPaginationHeader(user.CurrentPage, user.PageSize, user.TotalCount, user.TotalPages);
            return Ok(user);
        }

        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDTO>> GetUser(string username)
        {
            var isCurrentUser = (User.GetUserName() == username);
            return await _mainRespositories.AppUserRespository.GetMemberByUserNameAsync(username, isCurrentUser);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(UpdateMemberDTO member)
        {
            var user = await _mainRespositories.AppUserRespository.GetUserByUserNameAsync(User.GetUserName());

            _mapper.Map(member, user);
            _mainRespositories.AppUserRespository.Update(user);

            if (await _mainRespositories.Complete()) return NoContent();

            return BadRequest("Fail to update");

        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            var user = await _mainRespositories.AppUserRespository.GetUserByUserNameAsync(User.GetUserName());

            ImageUploadResult result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                PublicId = result.PublicId,
                Url = result.SecureUrl.AbsoluteUri
            };
            user.Photos.Add(photo);
            if (await _mainRespositories.Complete())
            {
                return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDTO>(photo));
            }
            return BadRequest("Fail to add photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _mainRespositories.AppUserRespository.GetUserByUserNameAsync(User.GetUserName());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) NotFound();
            if (photo.isMain) BadRequest("Photo is already the main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.isMain);
            if (currentMain != null) currentMain.isMain = false;

            photo.isMain = true;
            _mainRespositories.AppUserRespository.Update(user);
            if (await _mainRespositories.Complete()) return NoContent();

            return BadRequest("Fail to set main photo");
        }
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _mainRespositories.AppUserRespository.GetUserByUserNameAsync(User.GetUserName());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();
            if (photo.isMain) return BadRequest("You cannot delete the main photo.");
            if (photo.PublicId != null)
            {
                DeletionResult result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photo);
            if (await _mainRespositories.Complete()) return NoContent();

            return BadRequest("Fail to delete photo");
        }
    }
}