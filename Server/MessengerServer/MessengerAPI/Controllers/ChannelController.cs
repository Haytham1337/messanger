using System.Threading.Tasks;
using Application.IServices;
using Application.Models.ChatDto.Requests;
using Application.Models.ConversationDto.Requests;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChannelController : ControllerBase
    {
        private readonly IConversationService _conversationService;
        private object _chatService;

        public ChannelController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpPost]
        public async Task<IActionResult> SubscribeForChannel([FromBody]AddConversationRequest request)
        {
            request.userId = HttpContext.GetUserId();

            await _conversationService.SubscribeForChannelAsync(request);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateChannel([FromBody]AddGroupRequest request)
        {
            request.UserId = HttpContext.GetUserId();

            await _conversationService.CreateGroupAsync(request);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> LeaveChannel([FromBody]LeaveGroupRequest request)
        {
            request.UserId = HttpContext.GetUserId();

            await this._conversationService.LeaveGroupAsync(request);

            return Ok();
        }
    }
}