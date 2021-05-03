using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _usermanager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(UserManager<AppUser> usermanager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
        {
            _signInManager = signInManager;
            _usermanager = usermanager;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterUserDTO userDTO)
        {
            if (await ExistUser(userDTO.Username)) return BadRequest("Username is taken.");
            var user = _mapper.Map<AppUser>(userDTO);
            user.CreatedDate = DateTime.UtcNow;
            user.UserName = userDTO.Username.ToLower();
            var result = await _usermanager.CreateAsync(user, userDTO.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            result = await _usermanager.AddToRoleAsync(user, "Member");
            if(!result.Succeeded) return BadRequest(result.Errors);

            return new UserDTO
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> login(LoginUserDTO loginUserDTO)
        {
            var user = await _usermanager.Users.Include(x => x.Photos).IgnoreQueryFilters().SingleOrDefaultAsync(u => u.UserName.ToLower() == loginUserDTO.Username.ToLower());

            if (user == null) return Unauthorized("Invalid username");
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginUserDTO.Password, false );
            if(!result.Succeeded) return Unauthorized();
            
            return new UserDTO
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                PhotoUrl = user.Photos.FirstOrDefault(x => x.isMain)?.Url,
                Gender = user.Gender
            };
        }
        private Task<bool> ExistUser(string username)
        {
            return _usermanager.Users.AnyAsync(u => u.UserName == username.ToLower());
        }

    }
}