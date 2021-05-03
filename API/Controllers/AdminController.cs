using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Extensions;
using API.Helper;
using API.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _usermanager;
        private readonly IRespositories _mainRespository;
        public AdminController(UserManager<AppUser> usermanager, IRespositories mainRespository)
        {
            _mainRespository = mainRespository;
            _usermanager = usermanager;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _usermanager.Users
                .Include(u => u.UserRoles)
                .ThenInclude(r => r.Role)
                .OrderBy(u => u.UserName)
                .Select(u => new
                {
                    u.Id,
                    username = u.UserName,
                    roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
                })
                .ToListAsync();
            return Ok(users);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditUserRoles(string username, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(",");
            var user = await _usermanager.FindByNameAsync(username);
            if (user == null) NotFound("user does not exist");

            var currentRoles = await _usermanager.GetRolesAsync(user);
            var result = await _usermanager.AddToRolesAsync(user, selectedRoles.Except(currentRoles));

            if (!result.Succeeded) BadRequest(result.Errors);

            result = await _usermanager.RemoveFromRolesAsync(user, currentRoles.Except(selectedRoles));
            if (!result.Succeeded) BadRequest(result.Errors);

            return Ok(await _usermanager.GetRolesAsync(user));
        }

        [Authorize(Policy = "RequirePhotoModeratorRole")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult> GetUsersWithPhotos()
        {
            var members = await _mainRespository.AppUserRespository.GetMemberUnApprovedPhotos();
            return Ok(members);
        }
        [HttpPost("approve-photos")]
        public async Task<ActionResult> ApproveMembersPhotos(string[] photoIds){
            var users = await _mainRespository.AppUserRespository.GetUsersAsync();
            var photos = users.SelectMany(x => x.Photos.Where(p => photoIds.Contains(p.Id.ToString())));
            foreach(var photo in photos){
                photo.isApproved = true;
            } 
            if(await _mainRespository.Complete()) return Ok();

            return BadRequest("Fail to approve photos.");
        }
         
         [Authorize(Policy = "RequirePhotoModeratorRole")]
         [HttpPost("approve-photo/{photoId}")]
        public async Task<ActionResult> ApproveMemberPhoto(int photoId){
            if(await moderatePhoto(photoId, true)) return Ok();    
            return BadRequest("Fail to moderate photos.");
        }

         [Authorize(Policy = "RequirePhotoModeratorRole")]
         [HttpPost("reject-photo/{photoId}")]
        public async Task<ActionResult> RejectMemberPhoto(int photoId){
            if(await moderatePhoto(photoId, false)) return Ok();    
            return BadRequest("Fail to moderate photos.");
        }

        private async Task<bool> moderatePhoto(int photoId, bool isApproved){
            var users = await _mainRespository.AppUserRespository.GetUsersAsync();
            var photo = users.SelectMany(x => x.Photos.Where(p => p.Id == photoId)).FirstOrDefault<Photo>();
            if(photo != null){
                photo.isApproved = isApproved;
                photo.moderateDate = DateTime.UtcNow;
                photo.moderatedBy = User.GetUserName();
                //set photo as main if there is no main photo for member
                if(isApproved && users.Where(u => u.Id== photo.AppUserId && u.Photos.Any(p => p.isMain)).Count() <= 0)
                    photo.isMain=true;
                return await _mainRespository.Complete();
            }
            return false;
        }
    }
}