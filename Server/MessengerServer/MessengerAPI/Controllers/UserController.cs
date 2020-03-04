using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.IServices;
using Application.Models.ChatDto.Requests;
using Application.Models.PhotoDto;
using Application.Models.UserDto;
using Application.Models.UserDto.Requests;
using AutoMapper;
using Infrastructure.Extensions;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UserInfo()
        {
            var userInfo=await this._userService.GetUserInfoAsync(new GetUserInfoRequest() 
            { 
                UserName = User.Identity.Name 
            });

            return Ok(userInfo);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateUser(UpdateUserDto model)
        {
            model.UserId = HttpContext.GetUserId();

            await _userService.UpdateUserAsync(model);

            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<List<SearchUserDto>> Search([FromQuery]SearchRequest request )
        {
           request.UserId = HttpContext.GetUserId();

            return await this._userService.SearchUserAsync(request);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> BlockUser([FromBody]BlockUserRequest request)
        {
            request.UserId = HttpContext.GetUserId();

            await this._userService.BlockUserAsync(request);

            return Ok();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UnBlockUser([FromBody]BlockUserRequest request)
        {
            request.UserId = HttpContext.GetUserId();

            await this._userService.UnBlockUserAsync(request);

            return Ok();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePhoto(IFormCollection collection)
        {
            if (ModelState.IsValid && collection.Files[0] != null)
            {
                await _userService.ChangePhotoAsync(new AddPhotoDto()
                {
                    UserId = HttpContext.GetUserId(),
                    UploadedFile = collection.Files[0]
                });

                return Ok();
            }

            return BadRequest();
        }
    }
}