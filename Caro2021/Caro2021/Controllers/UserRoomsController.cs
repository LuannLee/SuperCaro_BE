using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Caro2021;
using Caro2021.Models;

namespace Caro2021.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoomsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserRoomsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/UserRooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserRoom>>> GetUserRooms()
        {
            return await _context.UserRooms.ToListAsync();
        }

        // GET: api/UserRooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserRoom>> GetUserRoom(Guid id)
        {
            var userRoom = await _context.UserRooms.FindAsync(id);

            if (userRoom == null)
            {
                return NotFound();
            }

            return userRoom;
        }

        // PUT: api/UserRooms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserRoom(Guid id, UserRoom userRoom)
        {
            if (id != userRoom.Id)
            {
                return BadRequest();
            }

            _context.Entry(userRoom).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserRoomExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserRooms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserRoom>> PostUserRoom(UserRoom userRoom)
        {
            if (string.IsNullOrEmpty(userRoom.UserId.ToString()) || string.IsNullOrEmpty(userRoom.RoomId.ToString()))
                return BadRequest("Bạn không thể truy cập phòng này");

            var user = _context.Users.FirstOrDefault(x => x.Id == userRoom.UserId);
            var room = _context.Rooms.FirstOrDefault(x => x.Id == userRoom.RoomId);

            if (null == user || null == room)
                return BadRequest("Bạn không thể truy cập phòng này");

            var currentUserRoom = _context.UserRooms.FirstOrDefault(x => x.UserId == userRoom.UserId && x.RoomId == userRoom.RoomId);

            if(null != currentUserRoom)
                return CreatedAtAction("GetUserRoom", new { id = currentUserRoom.Id }, currentUserRoom);

            userRoom.Id = Guid.NewGuid();

            _context.UserRooms.Add(userRoom);

            if (room.Status == 1)
                room.Status = 2;

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserRoom", new { id = userRoom.Id }, userRoom);
        }

        // DELETE: api/UserRooms/userId/roomId
        [HttpDelete("{userId}/{roomId}")]
        public async Task<IActionResult> DeleteUserRoom(string userId, string roomId)
        {
            var userroom = await _context.UserRooms.FirstOrDefaultAsync(x => x.UserId.ToString() == userId && x.RoomId.ToString() == roomId);

            if(null == userroom)
            {
                return BadRequest("Đối tượng chưa vào phòng");
            }

            _context.UserRooms.Remove(userroom);
            _context.SaveChanges();

            var UserRooms =  _context.UserRooms.Where(x => x.RoomId.ToString() == roomId);
            if(UserRooms.Count() == 0)
            {
                var room = _context.Rooms.FirstOrDefault(x => x.Id.ToString() == roomId);
                room.Status = 1;

                _context.Rooms.Update(room);
                _context.SaveChanges();
            }

           
            

            return Ok("Xoá thành công !");

        }

        private bool UserRoomExists(Guid id)
        {
            return _context.UserRooms.Any(e => e.Id == id);
        }
    }
}
