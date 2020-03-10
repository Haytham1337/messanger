using System.Collections.Generic;
using System.Threading.Tasks;
using Application.IServices;
using Application.Models.ChatDto.Requests;
using Application.Models.ChatDto.Responces;
using Application.Models.ConversationDto.Requests;
using Application.Models.ConversationDto.Responces;
using Application.Models.PhotoDto;
using Application.Models.UserDto;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService _conversationService;

        public ConversationController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody]AddConversationRequest request)
        {
            request.userId= HttpContext.GetUserId();

            await _conversationService.CreateChatAsync(request);

            return Ok();
        }

        [HttpGet]
        public async Task<List<GetConversationDto>> GetConversations([FromQuery]GetChatsRequestDto request)
        {
            request.UserName = User.Identity.Name;

            return await _conversationService.GetConversationsAsync(request);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeConversationPhoto(IFormCollection collection,[FromQuery(Name ="chatId")] int chatId)
        {
            if (ModelState.IsValid && collection.Files[0] != null)
            {
                await _conversationService.ChangePhotoAsync(new AddPhotoDto()
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
        public async Task<List<SearchConversationResponce>> Search([FromQuery]SearchRequest request)
        {
            request.UserId = HttpContext.GetUserId();

            return await this._conversationService.SearchConversation(request);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]DeleteRequest request)
        {
            request.UserId = HttpContext.GetUserId();

            await this._conversationService.DeleteConversationAsync(request);

            return Ok();
        }
    }
}