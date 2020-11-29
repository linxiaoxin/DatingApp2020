using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterUserDTO userDTO)
        {
            if (await ExistUser(userDTO.Username)) return BadRequest("Username is taken.");
            var user = _mapper.Map<AppUser>(userDTO);
            using var hmac = new HMACSHA512();

            user.UserName = userDTO.Username.ToLower();
            user.PasswordSalt = hmac.Key;
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userDTO.Password.ToLower()));

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDTO { 
                Username = user.UserName, 
                Token = _tokenService.CreateToken(user.UserName.ToLower()), 
                KnownAs = user.KnownAs };
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> login(LoginUserDTO loginUserDTO)
        {
            var user = await _context.Users.Include(x => x.Photos).SingleOrDefaultAsync(u => u.UserName.ToLower() == loginUserDTO.Username.ToLower());

            if (user == null) return Unauthorized("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginUserDTO.Password));

            for (int i = 0; i < computeHash.Length; i++)
            {
                if (computeHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }

            return new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user.UserName.ToLower()),
                KnownAs = user.KnownAs,
                PhotoUrl = user.Photos.FirstOrDefault(x => x.isMain)?.Url
            };
        }
        private Task<bool> ExistUser(string username)
        {
            return _context.Users.AnyAsync(u => u.UserName == username.ToLower());
        }

    }
}