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
            return await _context.Users.Include(u => u.Photos).IgnoreQueryFilters().ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int Id)
        {
            return await _context.Users.FindAsync(Id);
        }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            return await _context.Users.Include(x => x.Photos).IgnoreQueryFilters().SingleOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower());
        }

        public async Task<MemberDTO> GetMemberByUserNameAsync(string username, bool isCurrentUser)
        {
            var member = _context.Users
                        .Where(u => u.UserName.ToLower() == username.ToLower() )
                        .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
                        .AsQueryable();

            if(isCurrentUser) member = member.IgnoreQueryFilters();
            return await member.SingleOrDefaultAsync();
        }

        public async Task<PageList<MemberDTO>> GetMembersAsync(UserParams userParams)
        {
            var minDOB = System.DateTime.Today.AddYears(-(userParams.MaxAge+1));
            var maxDOB = System.DateTime.Today.AddYears(-userParams.MinAge);
            var query = _context.Users.AsQueryable()
                        .Where(x => x.DateOfBirth >= minDOB
                                && x.DateOfBirth <= maxDOB
                                && x.UserName.ToLower() != userParams.UserName.ToLower()
                                && (string.IsNullOrEmpty(userParams.Gender) || x.Gender.ToLower() == userParams.Gender.ToLower()))
                        .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
                        .AsNoTracking();            
            //sort By
            query = userParams.OrderBy switch{
                "created" =>query.OrderByDescending(x=> x.CreatedDate),
                "age" => query.OrderBy(x => x.Age),
                _ => query.OrderByDescending(x => x.LastActive) 
            };
            return await PageList<MemberDTO>.CreateAsync(query, userParams.PageSize, userParams.PageNumber);
        }

        
        public void Update(AppUser user){
           _context.Users.Update(user);
        }

        public async Task<List<PhotoForModerationDTO>> GetMemberUnApprovedPhotos()
        {
            var users = await _context.Users.IgnoreQueryFilters()
            .Include(x => x.Photos.Where(p=> p.moderateDate == null))
            .ToListAsync();

            var photos = new List<PhotoForModerationDTO>();

            foreach(var user in users){
              photos.AddRange( user.Photos.Select(p => new PhotoForModerationDTO{
                  id = p.Id,
                  url = p.Url,
                  isApproved = p.isApproved,
                  moderateDate = p.moderateDate,
                  userName = user.UserName,
                  knownAs = user.KnownAs  
              }));                 
            }
            
            return photos;
        }

    }
}