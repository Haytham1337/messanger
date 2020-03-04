using System.Collections.Generic;
using System.Threading.Tasks;
using Application.IServices;
using Application.Models.ChatDto.Requests;
using Application.Models.ChatDto.Responces;
using Application.Models.ConversationDto.Requests;
using Application.Models.ConversationDto.Responces;
using Application.Models.PhotoDto;
using Application.Models.UserDto;
using Infrastructure.AppSecurity;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IConversationService _chatService;

        public ChatController(IConversationService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody]AddConversationRequest request)
        {
            request.userId= HttpContext.GetUserId();

            await _chatService.CreateChatAsync(request);

            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<List<GetConversationDto>> GetChats([FromQuery]GetChatsRequestDto request)
        {
            request.UserName = User.Identity.Name;

            return await _chatService.GetChatsAsync(request);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateGroup([FromBody]AddGroupRequest request)
        {
            request.UserId = HttpContext.GetUserId();

            await _chatService.CreateGroupAsync(request);

            return Ok();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToGroup([FromBody]AddConversationRequest request)
        {
            request.userId =HttpContext.GetUserId();

            await _chatService.AddToGroup(request);

            return Ok();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangeGroupPhoto(IFormCollection collection,[FromQuery(Name ="chatId")] int chatId)
        {
            if (ModelState.IsValid && collection.Files[0] != null)
            {
                await _chatService.ChangePhotoAsync(new AddPhotoDto()
                {
                    ConversationId=chatId,
                    UserId = HttpContext.GetUserId(),
                UploadedFile = collection.Files[0]
                });

                return Ok();
            }

            return BadRequest();
        }

        [HttpGet]
        [Authorize]
        public async Task<List<SearchConversationResponce>> Search([FromQuery]SearchRequest request)
        {
            request.UserId = HttpContext.GetUserId();

            return await this._chatService.SearchConversation(request);
        }
    }
}