using Application.Models.ChatDto.Requests;
using Application.Models.ConversationDto.Requests;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IGroupService
    {
        Task LeaveGroupAsync(LeaveGroupRequest request);

        Task CreateGroupAsync(AddGroupRequest request);

        Task SubscribeAsync(AddConversationRequest request);

        Task AddConversationMemberAsync(AddConversationMemberRequest request);
    }
}
