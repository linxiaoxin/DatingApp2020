using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interface;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class AppUserRespository : IAppUserRespository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public AppUserRespository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<List<AppUser>> GetUsersAsync()
        {
            return await _context.Users.Include(u => u.Photos).ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int Id)
        {
            return await _context.Users.FindAsync(Id);
        }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower());
        }

        public async Task<MemberDTO> GetMemberByUserNameAsync(string username)
        {
            var member = await _context.Users
                        .Where(u => u.UserName.ToLower() == username.ToLower())
                        .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
                        .SingleOrDefaultAsync();

            return member;
        }

        public async Task<IEnumerable<MemberDTO>> GetMembersAsync()
        {
            var members = await _context.Users
                        .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
                        .ToListAsync();

            return members;
        }
    }
}