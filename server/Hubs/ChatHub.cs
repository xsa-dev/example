using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace WebApi.Hubs {
    public class ChatHub : Hub {
        public async Task SendMessage (string user, string message) {
            await Clients.All.SendAsync ("ReceiveMessage", user, message);
        }

        public async Task send () {
            await Clients.All.SendAsync("Hello");
        }
    }
}