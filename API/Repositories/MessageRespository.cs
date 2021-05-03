using System;
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
    public class MessageRespository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessageRespository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task AddGroup(MsgGroup group)
        {
            await _context.MsgGroup.AddAsync(group);
        }

        public async Task AddMessage(Message message)
        {
            await _context.Messages.AddAsync(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> FindMessage(int id)
        {
           return await _context.Messages.FindAsync(id);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connection.FindAsync( connectionId);
        }

        public async Task<PageList<MessageDTO>> GetMessageByUserName(MessageParams msgParams)
        {
            var query = _context.Messages
                        .ProjectTo<MessageDTO>(_mapper.ConfigurationProvider)
                        .OrderByDescending(m => m.DateSent).AsQueryable();

            query = msgParams.Container switch{
             "Inbox" => query.Where(m => m.RecipientUserName.ToLower() == msgParams.UserName.ToLower() && !m.RecipientDeleted),
             "Outbox" => query.Where(m => m.SenderUserName.ToLower() == msgParams.UserName.ToLower() && !m.SenderDeleted),
             _ =>  query.Where(m => m.RecipientUserName.ToLower() == msgParams.UserName.ToLower() && m.DateRead == null && !m.RecipientDeleted)  
            };

            return await PageList<MessageDTO>.CreateAsync(query,msgParams.PageSize,msgParams.PageNumber);
        }

        public async Task<MsgGroup> GetMessageGroup(string groupname)
        {
            return await _context.MsgGroup.Include(x=> x.Connections).FirstOrDefaultAsync(x=> x.Name==groupname);
        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread(string thisUserName, string otherUserName)
        {
            var msgs = await _context.Messages
                .Where(m => (m.RecipientUserName.ToLower() == thisUserName.ToLower() && m.SenderUserName.ToLower() == otherUserName.ToLower()
                            && !m.RecipientDeleted)
                        || (m.RecipientUserName.ToLower() == otherUserName.ToLower() && m.SenderUserName.ToLower() == thisUserName.ToLower()
                            && !m.SenderDeleted)
                        )
                .OrderBy(m => m.DateSent)
                .ProjectTo<MessageDTO>(_mapper.ConfigurationProvider).AsTracking()
                .ToListAsync();

            var UnReadmsg = msgs.Where(m => m.RecipientUserName.ToLower() == thisUserName.ToLower() && m.DateRead == null).ToList();
            UnReadmsg.ForEach(m =>{
               m.DateRead = DateTime.UtcNow;         
            });
            await _context.SaveChangesAsync();
            return msgs;
        }

        public async Task<MsgGroup> GetMsgGroupForConnection(string connectionId)
        {
            return await _context.MsgGroup
                .Include(x => x.Connections)
                .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId)).FirstOrDefaultAsync();
        }

        public void RemoveConnectio(Connection connection)
        {
           _context.Connection.Remove(connection);
        }

    }
}