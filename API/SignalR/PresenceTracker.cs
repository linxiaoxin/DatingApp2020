using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class PresenceTracker
    {
        public static readonly Dictionary<string, List<string>> onlineUsers = new Dictionary<string, List<string>>(); //username, connection id

        public Task<bool> AddOnlineUser(string username, string connectionid){
            bool isFirstConnected = false;
            lock (onlineUsers)
            {
                if(onlineUsers.ContainsKey(username))
                    onlineUsers[username].Add(connectionid);
                else{
                    onlineUsers.Add(username, new List<string>{connectionid});
                    isFirstConnected = true;
                }
            }
            return Task.FromResult(isFirstConnected);
        }

        public Task<bool> RemoveOnlineUser(string username, string connectionid){
            bool isOffline = false;
            lock(onlineUsers)
            {
                if(onlineUsers.ContainsKey(username)){
                    onlineUsers[username].Remove(connectionid);
                    if(onlineUsers[username].Count <= 0){
                        onlineUsers.Remove(username);
                        isOffline = true;
                    }
                }
            }
            return Task.FromResult(isOffline);
        }

        public Task<string[]> GetOnlineUsers(){
            string [] result;
            lock(onlineUsers){

                result= onlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();    
            }
            return Task.FromResult(result);
        }

        public Task<List<string>> GetConnectionIdforOnlineUser(string username){
            List<string> connectionIds;
            lock(onlineUsers){
                connectionIds = onlineUsers.GetValueOrDefault(username);
            }
            return Task.FromResult(connectionIds);
        }
    }
}