using Application.IServices;
using Application.Models.ChatDto.Requests;
using Application.Models.ConversationDto.Requests;
using Domain;
using Domain.Entities;
using Domain.Exceptions.ChatExceptions;
using Domain.Exceptions.ConversationExceptions;
using Domain.Exceptions.UserExceptions;
using Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class GroupService : IGroupService
    {
        private readonly IUnitOfWork _unit;

        private readonly IConfiguration _config;

        private readonly IAuthService _auth;

        public GroupService(IUnitOfWork unit, IConfiguration conf, IAuthService auth)
        {
            _unit = unit;

            _config = conf;

            _auth = auth;
        }
        public async Task LeaveGroupAsync(LeaveGroupRequest request)
        {
            var conversation = await _unit.ConversationRepository.GetWithUsersConversationsAsync(request.ConversationId);

            if (conversation == null || conversation.Type == ConversationType.Chat)
                throw new ConversationNotExistException(ExceptionMessages.ConversationNotExist, 400);

            var userConversation = conversation.UserConversations.FirstOrDefault(uconv => uconv.UserId == request.UserToLeaveId);

            if (userConversation == null)
                throw new UserConversationNotExistException(ExceptionMessages.NotMember, 400);

            if (request.UserId == request.UserToLeaveId)
            {
                if (conversation.ConversationInfo.AdminId == request.UserId)
                {
                    var newAdmin = conversation.UserConversations.Where(uconv => uconv.UserId != request.UserId).FirstOrDefault();

                    if (newAdmin == null)
                        throw new UserNotExistException(ExceptionMessages.UserNotExist, 400);

                    conversation.ConversationInfo.AdminId = newAdmin.UserId;

                    this._unit.ConversationInfoRepository.Update(conversation.ConversationInfo);
                }

                await _unit.UserConversationRepository.DeleteAsync(userConversation.Id);
            }
            else
            {
                if (conversation.ConversationInfo.AdminId == request.UserId)
                    await _unit.UserConversationRepository.DeleteAsync(userConversation.Id);
                else
                    throw new UserNotHaveRigthsException(ExceptionMessages.NotHaveRigths, 400);
            }

            await _unit.Commit();
        }

        public async Task CreateGroupAsync(AddGroupRequest request)
        {
            var currentUser = await _auth.FindByIdUserAsync(request.UserId);

            if (currentUser == null)
                throw new UserNotExistException(ExceptionMessages.UserNotExist, 400);

            request.UsersId.Add(currentUser.Id);

            var grettingMessage = new Message()
            {
                Content = _config.GetValue<string>("greetmessage"),
                TimeCreated = DateTime.Now,
                UserId = currentUser.Id
            };

            var conversationInfo = new ConversationInfo
            {
                AdminId = currentUser.Id,
                PhotoName = _config.GetValue<string>("defaultgroup"),
                GroupName = request.GroupName
            };

            var conversation = new Conversation
            {
                Type = request.IsChannel == true ? ConversationType.Channel : ConversationType.Group,

                ConversationInfo = conversationInfo,

                LastMessage = grettingMessage
            };

            await _unit.ConversationRepository.CreateAsync(conversation);

            await _unit.ConversationInfoRepository.CreateAsync(conversationInfo);

            var usersExistList = await this._unit.UserRepository.GetUsersIn(request.UsersId);

            foreach (var user in usersExistList)
            {
                await _unit.UserConversationRepository.CreateAsync(new UserConversation
                {
                    UserId = user.Id,
                    Conversation = conversation
                });
            }

            await _unit.Commit();
        }

        public async Task SubscribeAsync(AddConversationRequest request)
        {
            var conversation = await _unit.ConversationRepository.GetWithUsersConversationsAsync(request.id);

            if (conversation.Type != ConversationType.Channel)
                throw new ConversationNotExistException(ExceptionMessages.InvalidConversationType, 400);

            if (conversation.UserConversations.Any(uconv => uconv.UserId == request.userId))
                throw new UserAlreadyExistException(ExceptionMessages.AlreadyMember, 400);

            var userConversation = new UserConversation
            {
                ConversationId = conversation.Id,
                UserId = request.userId
            };

            await _unit.UserConversationRepository.CreateAsync(userConversation);

            await _unit.Commit();
        }

        public async Task AddConversationMemberAsync(AddConversationMemberRequest request)
        {
            var conversation = await _unit.ConversationRepository.GetWithUsersConversationsAsync(request.ConversationId);

            if (conversation == null || conversation.Type != ConversationType.Group)
                throw new ConversationNotExistException(ExceptionMessages.ConversationNotExist, 400);

            var userConversation = conversation.UserConversations.FirstOrDefault(uconv => uconv.UserId == request.UserId);

            if (userConversation == null)
                throw new UserConversationNotExistException(ExceptionMessages.NotMember, 400);

            var userToAddConversation = conversation.UserConversations.FirstOrDefault(uconv => uconv.UserId == request.UserToAdd);

            if (userToAddConversation == null)
            {
                userToAddConversation = new UserConversation
                {
                    UserId = request.UserToAdd,
                    ConversationId = request.ConversationId
                };

                await _unit.UserConversationRepository.CreateAsync(userToAddConversation);

                await _unit.Commit();
            }
            else
            {
                throw new UserAlreadyExistException(ExceptionMessages.AlreadyMember, 400);
            }
        }
    }
}
