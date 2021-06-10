using Caro2021.HubConfig;
using Caro2021.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caro2021.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly AppDbContext _context;
        private IHubContext<CaroRealtimeHub> _hub;

        public ChatController(AppDbContext context, IHubContext<CaroRealtimeHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        [HttpPost("message-user")]
        public IActionResult MessageUser(MessageUser messageUser)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == messageUser.UserId);
            if(null == user)
            {
                return BadRequest("Không tìm thấy User");
            }

            var room = _context.Rooms.FirstOrDefault(x => x.Id == messageUser.RoomId);

            if(null == room)
            {
                return BadRequest("Không tìm thấy phòng");
            }

            // Gửi thông điệp trong SignalR

            _hub.Clients.All.SendAsync("chat-online", messageUser);

            return Ok();
        }
    }
}
