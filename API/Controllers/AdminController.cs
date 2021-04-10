using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _usermanager;
        public AdminController(UserManager<AppUser> usermanager)
        {
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
                .Select( u => new {
                    u.Id,
                    username = u.UserName,
                    roles = u.UserRoles.Select( ur => ur.Role.Name).ToList()    
                })
                .ToListAsync();    
            return Ok(users);
        }

        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditUserRoles(string username, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(",");
            var user = await _usermanager.FindByNameAsync(username);
            if(user == null) NotFound("user does not exist");

            var currentRoles = await _usermanager.GetRolesAsync(user);
            var result = await _usermanager.AddToRolesAsync(user, selectedRoles.Except(currentRoles));

            if(!result.Succeeded) BadRequest(result.Errors);

            result = await _usermanager.RemoveFromRolesAsync(user, currentRoles.Except(selectedRoles));
            if(!result.Succeeded) BadRequest(result.Errors);

            return Ok(await _usermanager.GetRolesAsync(user));
        }
        [Authorize(Policy = "RequirePhotoModeratorRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetUsersWithPhotos()
        {
            return Ok("Only admin and moderator can see this.");
        }
    }
}