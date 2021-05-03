using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Helper;

namespace API.Interface
{
    public interface IAppUserRespository
    {
        Task<List<AppUser>> GetUsersAsync();

        Task<AppUser> GetUserByUserNameAsync(string username);

        Task<AppUser> GetUserByIdAsync(int Id);

        Task<MemberDTO> GetMemberByUserNameAsync(string username, bool isCurrentUser);

        Task<PageList<MemberDTO>> GetMembersAsync(UserParams userParams);

        Task<List<PhotoForModerationDTO>> GetMemberUnApprovedPhotos();

        void Update(AppUser user);

    }
}