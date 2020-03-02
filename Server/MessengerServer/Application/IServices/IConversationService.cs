using Application.Models.ChatDto.Requests;
using Application.Models.ChatDto.Responces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IConversationService
    {
        Task CreateChatAsync(AddChatRequest request);

        Task<List<GetChatDto>> GetChatsAsync(GetChatsRequestDto request);
    }
}
