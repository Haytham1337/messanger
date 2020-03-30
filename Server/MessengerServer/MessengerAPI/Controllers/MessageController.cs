using System.Threading.Tasks;
using Application.IServices;
using Application.Models.ChatDto.Requests;
using Application.Models.MessageDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;

        }

        [HttpGet]
        [Authorize]
        public async Task<AllMessagesDto> GetConversationMessages([FromQuery]GetChatMessagesRequest request)
        {
            var responce = await this._messageService.GetMessageByChatAsync(request);

            return responce;
        }

        [HttpPost]
        public async Task<IActionResult> ChangeConversationPhoto(IFormCollection collection)
        {
            if (collection.Files[0] != null)
            {
                var photoPath=await _messageService.SaveMessagePhotoAsync(collection.Files[0]);
                 
                return Ok(photoPath);
            }

            return BadRequest();
        }
    }
}