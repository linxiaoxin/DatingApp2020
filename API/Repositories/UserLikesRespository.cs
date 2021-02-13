using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using API.Helper;
using API.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    [Authorize]
    public class UserLikesRespository : IUserLikesRespository
    {
        private readonly DataContext _context;
        public UserLikesRespository(DataContext context)
        {
            _context = context;
        }

        public async Task<PageList<LikesDTO>> GetLikes(LikesParams likesParams)
        {
            IQueryable<LikesDTO> result = null;
            if(likesParams.Predicate == "like"){
                result = _context.Users
                    .Include(u => u.Photos)
                    .Where(x => _context.Likes.Where(x => x.likedByUserId == likesParams.UserId).Select(x=>x.likedUserId).Contains(x.Id))
                    .Select(u => new LikesDTO{
                        UserName =  u.UserName,
                        Country = u.Country,
                        photoUrl = u.Photos.FirstOrDefault(x => x.isMain).Url,
                        Age = u.Age,
                        KnownAs = u.KnownAs,
                    });
            }
            if(likesParams.Predicate ==  "likeBy"){
                result = _context.Users
                    .Include(x => x.Photos)
                    .Where(x => _context.Likes.Where(x => x.likedUserId == likesParams.UserId).Select(x=>x.likedByUserId).Contains(x.Id))
                    .Select(u => new LikesDTO{
                        UserName =  u.UserName,
                        Country = u.Country,
                        photoUrl = u.Photos.FirstOrDefault(x => x.isMain).Url,
                        Age = u.Age,
                        KnownAs = u.KnownAs,
                    });
            }
            
            return await PageList<LikesDTO>.CreateAsync(result, likesParams.PageSize, likesParams.PageNumber);
        }

        public async Task<UserLikes> GetUserLikes(int likedByUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(likedByUserId, likedUserId);
        }

        public async Task<AppUser> GetUserWithLikes(int likedByUserId)
        {
           return await _context.Users.Include(x => x.LikedUsers).FirstOrDefaultAsync(x => x.Id == likedByUserId);
        }
    }
}