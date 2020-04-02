using Application.Models.ChatDto.Requests;
using Application.Models.MessageDto;
using Application.Models.MessageDto.Requests;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IMessageService
    {
        Task<GetMessageDto> AddMessageAsync(AddMessageDto message);

        Task<AllMessagesDto> GetMessageByChatAsync(GetChatMessagesRequest request);

        Task<string> SaveMessagePhotoAsync(IFormFile uploadedFile);

        Task DeleteMessageAsync(DeleteMessageRequest request);
    }
}
