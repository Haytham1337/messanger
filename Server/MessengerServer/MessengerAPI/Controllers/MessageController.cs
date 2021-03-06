using System.Threading.Tasks;
using Application.IServices;
using Application.Models.ChatDto.Requests;
using Application.Models.MessageDto;
using Application.Models.MessageDto.Requests;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;

        }

        [HttpGet]
        public async Task<AllMessagesDto> GetConversationMessages([FromQuery]GetChatMessagesRequest request)
        {
            var responce = await this._messageService.GetMessageByChatAsync(request);

            return responce;
        }

        [HttpPut]
        public async Task<string> LoadMessagePhoto(IFormCollection collection)
        {
            string photoName = string.Empty;

            if (collection.Files[0] != null)
            {
                photoName = await _messageService.SaveMessagePhotoAsync(collection.Files[0]);
            }

            return photoName;
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMessage([FromQuery]DeleteMessageRequest request)
        {
            request.UserId = HttpContext.GetUserId();

            await _messageService.DeleteMessageAsync(request);

            return Ok();
        }
    }
}