using Application.Models.ChatDto.Requests;
using Application.Models.ChatDto.Responces;
using Application.Models.ConversationDto.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IConversationService
    {
        Task CreateChatAsync(AddChatRequest request);

        Task<List<GetConversationDto>> GetChatsAsync(GetChatsRequestDto request);

        Task CreateGroupAsync(AddGroupRequest request);
    }
}
