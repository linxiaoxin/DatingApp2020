using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly DataContext _context;
        public BuggyController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("server-error")]
        public ActionResult<string> ServerError()
        {
            var thing = _context.Users.Find(-1);
            var thingstring = thing.ToString();

            return thingstring;
        }

        [Authorize]
        [HttpGet("Auth")]
        public ActionResult<AppUser> Auth()
        {
            //var thing = _context.Users.Find(-1);
            return Unauthorized();
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> UserNotFound()
        {
            var thing = _context.Users.Find(-1);
            if (thing == null) return NotFound("User is not found");
            return thing;
        }

        [HttpGet("bad-request")]
        public ActionResult<AppUser> BadUserRequest()
        {
            return BadRequest();
        }

    }
}