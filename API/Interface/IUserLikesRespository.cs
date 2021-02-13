using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Helper;

namespace API.Interface
{
    public interface IUserLikesRespository
    {
        Task<UserLikes> GetUserLikes(int likedByUserId, int likedUserId);

        Task<AppUser> GetUserWithLikes(int likedByUserId);

        Task<PageList<LikesDTO>> GetLikes(LikesParams pageParams);
    }
}