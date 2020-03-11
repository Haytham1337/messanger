using Application.Models.ChatDto.Requests;
using Application.Models.ChatDto.Responces;
using Application.Models.ConversationDto.Requests;
using Application.Models.ConversationDto.Responces;
using Application.Models.PhotoDto;
using Application.Models.UserDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IGroupService
    {
        Task CreateChatAsync(AddConversationRequest request);

        Task<List<GetConversationDto>> GetConversationsAsync(GetChatsRequestDto request);

        Task ChangePhotoAsync(AddPhotoDto model);

        Task<List<SearchConversationResponce>> SearchConversation(SearchRequest request);

        Task DeleteConversationAsync(DeleteRequest request);
    }
}
