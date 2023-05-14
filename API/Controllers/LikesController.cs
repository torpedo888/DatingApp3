using System.Runtime.CompilerServices;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseController
    {
        private readonly IUnitOfWork _uow;
        
        public LikesController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            //aki like-ol a belogolot user
            var sourceUserId = User.GetUserId();

            //aki like-olva lett.
            var likedUser = await _uow.UserRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _uow.LikesRepository.GetUserWithLikes(sourceUserId);

            if(likedUser == null) return NotFound();

            //ha a belogolt user sajat magat akarna likolni:
            if(sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            var userLike = await _uow.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);

            if(userLike!=null) return BadRequest("You already like this user");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            if(await _uow.Complete()) return Ok();

            return BadRequest("Failed to like the user");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();   

            var users = await _uow.LikesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, 
                users.PageSize, users.TotalCount, users.TotalPages));

            return Ok(users);
        }
    }
}