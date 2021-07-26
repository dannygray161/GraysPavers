using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraysPavers_Utility;
using Microsoft.AspNetCore.SignalR;

namespace GraysPavers.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
