using Caro2021.HubConfig;
using Caro2021.Models;
using Caro2021.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Caro2021.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IHubContext<CaroRealtimeHub> _hub;
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        public UserController(AppDbContext context, IConfiguration config, IHubContext<CaroRealtimeHub> hub)
        {
            _context = context;
            _config = config;
            _hub = hub;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register(RegisterUser request)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userDb = _context.Users.FirstOrDefault(x => x.UserName == request.UserName);
            if(userDb != null)
            {
                return BadRequest("User Name này đã được đăng ký !!!");
            }
            if(request.Password != request.ConfirmPassword)
            {
                return BadRequest("Mật khẩu xác nhận không đúng !!");
            }

            

            var user = new User();
            user.Id = Guid.NewGuid();
            user.IsAdmin = request.IsAdmin;
            user.Name = request.Name;
            user.UserName = request.UserName;
            user.Password = request.Password;

            user.Score = 100;

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("Đăng ký tài khoản thành công !!!");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login(LoginUser request)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName == request.UserName);

            if(user == null)
            {
                return BadRequest("Tài khoản này chưa được đăng ký !!!");
            }

            if(user.Password != request.Password)
            {
                return BadRequest("Mật khẩu không chính xác !");
            }

            // Ở đây chúng ta nhận ra rằng user tồn tại và đúng mật khẩu

            // thực hiện xử lý cập nhật status bằng true

            user.Status = true;
            _context.Update(user);
            _context.SaveChanges();


            var claims = new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("useName", user.UserName),
                new Claim("password", user.Password),
                new Claim("name", user.Name),
                new Claim("isAdmin", user.IsAdmin.ToString()),
                new Claim("score", user.Score.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                _config["Tokens:Issuer"],
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );

            var users = _context.Users.ToList();

            // Gửi danh sách User đi
            _hub.Clients.All.SendAsync("user-online", users);

            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }

        [HttpPost("logout")]
        
        public IActionResult logout(LoginUser request)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName == request.UserName);

            if (user == null)
            {
                return BadRequest("Tài khoản này chưa được đăng ký !!!");
            }

            if (user.Password != request.Password)
            {
                return BadRequest("Mật khẩu không chính xác !");
            }

            // Ở đây chúng ta nhận ra rằng user tồn tại và đúng mật khẩu

            // thực hiện xử lý cập nhật status bằng true

            user.Status = false;
            _context.Update(user);
            _context.SaveChanges();


            var users = _context.Users.ToList();

            // Gửi danh sách User đi
            _hub.Clients.All.SendAsync("user-online", users);

            return Ok("Đăng xuất thành công !");
        }

        [HttpGet("get-user")]
        [AllowAnonymous]
        public IActionResult getuser()
        {
            var users = _context.Users.ToList();
            if (null == users || 0 == users.Count())
                return BadRequest("Không có danh sách người dùng");
            return Ok(users);
        }

    }
}
