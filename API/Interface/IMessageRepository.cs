using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Helper;

namespace API.Interface
{
    public interface IMessageRepository
    {
        Task<Message> FindMessage(int id);
        Task AddMessage(Message message);

        void DeleteMessage(Message message);

        Task<PageList<MessageDTO>> GetMessageByUserName(MessageParams msgParams);
        
        Task<IEnumerable<MessageDTO>> GetMessageThread(string thisUserName, string otherUserName);

        Task<bool> SaveAllAsync();
    }
}