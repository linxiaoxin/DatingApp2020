using System.Threading.Tasks;

namespace API.Interface
{
    public interface IRespositories
    {
         IAppUserRespository AppUserRespository{get;}

         IMessageRepository MessageRepository{get;}
         IUserLikesRespository UserLikesRespository{get;}

         Task<bool> Complete();

         bool HasChanges();
    }
}