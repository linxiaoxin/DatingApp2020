using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterUserDTO userDTO)
        {
            if (await ExistUser(userDTO.Username)) return BadRequest("Username is taken.");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = userDTO.Username.ToLower(),
                PasswordSalt = hmac.Key,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userDTO.Password.ToLower()))
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDTO{Username = user.UserName, Token= _tokenService.CreateToken(user.UserName.ToLower())};
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> login(LoginUserDTO loginUserDTO)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == loginUserDTO.Username.ToLower());

            if (user == null) return Unauthorized("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginUserDTO.Password));

            for (int i = 0; i < computeHash.Length; i++)
            {
                if (computeHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }

            return new UserDTO{Username = user.UserName, Token= _tokenService.CreateToken(user.UserName.ToLower())};
        }
        private Task<bool> ExistUser(string username)
        {
            return _context.Users.AnyAsync(u => u.UserName == username.ToLower());
        }

    }
}