using System;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _presenceTracker;
        public PresenceHub(PresenceTracker presenceTracker)
        {
            _presenceTracker = presenceTracker;
        }

        public override async Task OnConnectedAsync()
        {
            var username = this.Context.User.GetUserName();
            if(await this._presenceTracker.AddOnlineUser(username, this.Context.ConnectionId))
                await this.Clients.Others.SendAsync("UserIsOnline", username);

            var allOnlineUsers = await this._presenceTracker.GetOnlineUsers();
            await this.Clients.Caller.SendAsync("GetOnlineUsers", allOnlineUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var username = this.Context.User.GetUserName();
            if(await this._presenceTracker.RemoveOnlineUser(username, this.Context.ConnectionId))
                await this.Clients.Others.SendAsync("UserIsOffLine", username);

            await base.OnDisconnectedAsync(exception);
        }
    }
}