using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;

namespace API.Interface
{
    public interface IAppUserRespository
    {
        Task<List<AppUser>> GetUsersAsync();

        Task<AppUser> GetUserByUserNameAsync(string username);

        Task<AppUser> GetUserByIdAsync(int Id);

        Task<MemberDTO> GetMemberByUserNameAsync(string username);

        Task<IEnumerable<MemberDTO>> GetMembersAsync();

        void Update(AppUser user);

        Task<bool> SaveAllAsync();
    }
}