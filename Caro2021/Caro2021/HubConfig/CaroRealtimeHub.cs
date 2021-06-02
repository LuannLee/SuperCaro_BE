using Caro2021.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caro2021.HubConfig
{
    public class CaroRealtimeHub: Hub
    {

        public async Task UserOnline(List<User> users) => await Clients.All.SendAsync("user-online", users);
    }
}
