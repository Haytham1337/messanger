using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.IServices;
using Application.Models.ConversationDto.Requests;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private IConversationService _conversationService;

        public GroupController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewGroupMember([FromBody]AddConversationMemberRequest request)
        {
            request.UserId = HttpContext.GetUserId();

            await this._conversationService.AddConversationMemberAsync(request);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> LeaveGroup([FromBody]LeaveGroupRequest request)
        {
            request.UserId = HttpContext.GetUserId();

            await this._conversationService.LeaveGroupAsync(request);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody]AddGroupRequest request)
        {
            request.UserId = HttpContext.GetUserId();

            await _conversationService.CreateGroupAsync(request);

            return Ok();
        }
    }
}