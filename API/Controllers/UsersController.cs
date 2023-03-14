using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("getnums")]
        public ActionResult<List<int>> GetNums()
        {
            Stack<int> nums = new Stack<int>();

            for(int i=0; i <10;i++)
            {
                nums.Push(i);
            }

            List<int> reverseorderednums = new List<int>();

            int length = nums.Count();
            for(int i=0; i<length;i++)
            {
                reverseorderednums.Add(nums.Pop());
            }

            return  reverseorderednums;
        }
    }
}