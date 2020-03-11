using System.Threading.Tasks;
using Application.IServices;
using Application.Models.ChatDto.Requests;
using Application.Models.ConversationDto.Requests;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ChannelController : ControllerBase
    {
        private readonly IGroupService _groupService;
        public ChannelController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost]
        public async Task<IActionResult> SubscribeForChannel([FromBody]AddConversationRequest request)
        {
            request.userId = HttpContext.GetUserId();

            await _groupService.SubscribeForChannelAsync(request);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateChannel([FromBody]AddGroupRequest request)
        {
            request.UserId = HttpContext.GetUserId();

            await _groupService.CreateGroupAsync(request);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> LeaveChannel([FromBody]LeaveGroupRequest request)
        {
            request.UserId = HttpContext.GetUserId();

            await this._groupService.LeaveGroupAsync(request);

            return Ok();
        }
    }
}