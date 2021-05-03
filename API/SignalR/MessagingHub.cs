using System;
using System.Linq;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class MessagingHub : Hub
    {
        private readonly IMapper _mapper;
        private readonly PresenceTracker _presenceTracker;
        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly IRespositories _mainRespositores;

        public MessagingHub(IRespositories mainRespositores, IMapper mapper, PresenceTracker presenceTracker,
            IHubContext<PresenceHub> presenceHub)
        {
            _mainRespositores = mainRespositores;
            _presenceHub = presenceHub;
            _presenceTracker = presenceTracker;
            _mapper = mapper;
        }

        public override async Task OnConnectedAsync()
        {
            var username = Context.User.GetUserName();
            var otherusername = Context.GetHttpContext().Request.Query["user"];
            var groupname = this.GetGroupName(username, otherusername);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupname);
            var msgGroup = await RecordMsgGroup(groupname);

            var msgThread = await _mainRespositores.MessageRepository.GetMessageThread(username, otherusername);
            await Clients.Caller.SendAsync("AllMessageThread", msgThread);
            if(_mainRespositores.HasChanges()) await _mainRespositores.Complete();
            
            await Clients.OthersInGroup(groupname).SendAsync("UpdatedGroup", msgGroup);

        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            MsgGroup msgGroup = await _mainRespositores.MessageRepository.GetMsgGroupForConnection(Context.ConnectionId);
            Connection connection = msgGroup.Connections.Where(x => x.ConnectionId == Context.ConnectionId).FirstOrDefault();
            if (connection != null)
            {
                _mainRespositores.MessageRepository.RemoveConnectio(connection);
                if (await _mainRespositores.Complete())
                    await Clients.OthersInGroup(msgGroup.Name).SendAsync("UpdatedGroup", msgGroup);
            }
            await base.OnDisconnectedAsync(exception);
        }


        public async Task AddMessage(CreateMessageDTO message)
        {
            var sendUserName = Context.User.GetUserName();
            if (sendUserName == message.RecipientUserName) throw new HubException("Not allowed to send message to yourself");
            var sender = await _mainRespositores.AppUserRespository.GetUserByUserNameAsync(sendUserName);
            var recipient = await _mainRespositores.AppUserRespository.GetUserByUserNameAsync(message.RecipientUserName);

            if (recipient == null) new HubException("Receipient is not found");
            var groupname = this.GetGroupName(sender.UserName, recipient.UserName);

            var msgGroup = await _mainRespositores.MessageRepository.GetMessageGroup(groupname);

            var newMsg = new Message
            {
                SenderId = sender.Id,
                SenderUserName = sender.UserName,
                RecipientId = recipient.Id,
                RecipientUserName = recipient.UserName,
                Content = message.Content
            };

            if (msgGroup.Connections.Any(x => x.Username == message.RecipientUserName))
                newMsg.DateRead = DateTime.UtcNow;
            else
                await NotifyNewMsg(recipient.UserName, sender);

            await _mainRespositores.MessageRepository.AddMessage(newMsg);
            if (await _mainRespositores.Complete())
            {
                await Clients.Group(groupname).SendAsync("NewMessage", _mapper.Map<MessageDTO>(newMsg));
            }
            else
                throw new HubException("Fail to send message");
        }

        private string GetGroupName(string username, string otherusername)
        {
            return string.CompareOrdinal(username, otherusername) < 0 ? $"{username}-{otherusername}" : $"{otherusername}-{username}";
        }

        private async Task<MsgGroup> RecordMsgGroup(string groupname)
        {
            var msgGroup = await _mainRespositores.MessageRepository.GetMessageGroup(groupname);
            if (msgGroup == null)
            {
                msgGroup = new MsgGroup(groupname);
                await _mainRespositores.MessageRepository.AddGroup(msgGroup);
            }
            msgGroup.Connections.Add(new Connection(Context.ConnectionId, Context.User.GetUserName()));
            if (await _mainRespositores.Complete()) return msgGroup;
            throw new HubException("Fail to join message group");
        }
        private async Task NotifyNewMsg(string receipientusename, AppUser sender)
        {
            var clientConnection = await _presenceTracker.GetConnectionIdforOnlineUser(receipientusename);
            if (clientConnection != null)
                foreach (var connection in clientConnection)
                {
                    await _presenceHub.Clients.Client(connection).SendAsync("NotifyNewMsg", new { username = sender.UserName, knownAs = sender.KnownAs });
                }
        }
    }
}