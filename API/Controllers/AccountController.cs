using System.Security.Cryptography;
using System.Text;
using System.Windows.Markup;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Configuration;

namespace API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly DataContext _context;
        public ITokenService _tokenService { get; }

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            if(await UserExist(registerDto.UserName)) return  BadRequest("user exists");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                 UserName = registerDto.UserName.ToLower(),
                 PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                 PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x=>x.UserName == loginDto.UserName);

            if(user==null) return Unauthorized("invalid username");

            var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(var i=0;i<computedHash.Length;i++)
            {
                if(user.PasswordHash[i]!=computedHash[i]) return Unauthorized("invalid password");
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        private async Task<bool> UserExist(string userName)
        {
            return await _context.Users.AnyAsync(x=>x.UserName == userName.ToLower());
        }
    }
}