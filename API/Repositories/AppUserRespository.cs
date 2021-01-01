using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using API.Helper;
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
            return await _context.Users.Include(x => x.Photos).SingleOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower());
        }

        public async Task<MemberDTO> GetMemberByUserNameAsync(string username)
        {
            var member = await _context.Users
                        .Where(u => u.UserName.ToLower() == username.ToLower())
                        .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
                        .SingleOrDefaultAsync();

            return member;
        }

        public async Task<PageList<MemberDTO>> GetMembersAsync(UserParams userParams)
        {
            var minDOB = System.DateTime.Today.AddYears(-(userParams.MaxAge+1));
            var maxDOB = System.DateTime.Today.AddYears(-userParams.MinAge);

            var query = _context.Users.AsQueryable()
                        .Where(x => x.DateOfBirth >= minDOB
                                && x.DateOfBirth <= maxDOB
                                && x.UserName.ToLower() != userParams.UserName.ToLower());

            if(userParams.Gender != null &&  userParams.Gender != "")
                query = query.Where(x=> x.Gender == userParams.Gender);

            //sort By
            query = userParams.OrderBy switch{
                "created" =>query.OrderByDescending(x=> x.CreatedDate),
                "age" => query.OrderBy(x => x.DateOfBirth),
                _ => query.OrderByDescending(x => x.LastActive) 
            };
            return await PageList<MemberDTO>.CreateAsync(query.ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
                        .AsNoTracking(), userParams.PageSize, userParams.PageNumber);
        }

        public void Update(AppUser user){
           _context.Users.Update(user);
        }

        public async Task<bool> SaveAllAsync(){
           return await _context.SaveChangesAsync() > 0;
        }
    }
}