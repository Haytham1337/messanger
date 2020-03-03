using Application.IServices;
using Application.Models.ChatDto.Requests;
using Application.Models.ChatDto.Responces;
using Application.Models.ConversationDto.Requests;
using Application.Models.PhotoDto;
using Domain;
using Domain.Entities;
using Domain.Exceptions.ChatExceptions;
using Domain.Exceptions.UserExceptions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IUnitOfWork _unit;

        private readonly IAuthService _auth;

        private readonly IConfiguration _config;

        private readonly IPhotoHelper _photoHelper;

        public ConversationService(IUnitOfWork unit, IAuthService auth, IConfiguration config,IPhotoHelper photoHelper)
        {
            _unit = unit;

            _auth = auth;

            _config = config;

            _photoHelper = photoHelper;
        }

        public async Task CreateChatAsync(AddChatRequest request)
        {
            var user = await _auth.FindByIdUserAsync(request.userId);

            if (user == null)
                throw new UserNotExistException("user not exist", 400);

            if ((await this._unit.ConversationRepository.ChatExistAsync(user.Id, request.SecondUserId)))
            {
                var grettingMessage = new Message()
                {
                    Content = _config.GetValue<string>("greetmessage"),
                    TimeCreated = DateTime.Now,
                    UserId = user.Id,
                };

                var chat = new Conversation()
                {
                    Type = ConversationType.Chat,

                    LastMessage = grettingMessage
                };

                var firstUserConversation = new UserConversation
                {
                    UserId = user.Id,
                    Conversation = chat
                };

                var secondUserConversation = new UserConversation
                {
                    UserId = request.SecondUserId,
                    Conversation = chat
                };

                await this._unit.ConversationRepository.CreateAsync(chat);

                await this._unit.UserConversationRepository.CreateAsync(firstUserConversation);

                await this._unit.UserConversationRepository.CreateAsync(secondUserConversation);

                await this._unit.Commit();
            }
            else
            {
                throw new ChatAlreadyExistException("chat already exist", 400);
            }
        }

        public async Task<List<GetConversationDto>> GetChatsAsync(GetChatsRequestDto request)
        {
            var user = await this._unit.UserRepository.GetUserWithBlackList(request.UserName);

            if (user == null)
                throw new UserNotExistException("Given user not exist!", 400);

            var conversationList = await _unit.ConversationRepository.GetUserChatsAsync(user.Id);

            var res = new List<GetConversationDto>();

            foreach (var conversation in conversationList)
            {
                if (conversation.Type == ConversationType.Chat)
                {
                    var secondUserId = conversation.UserConversations[0].UserId == user.Id ? conversation.UserConversations[1].UserId :
                        conversation.UserConversations[0].UserId;

                    var secondUser = await _auth.FindByIdUserAsync(secondUserId);

                    res.Add(new GetConversationDto()
                    {
                        Id = conversation.Id,
                        Photo = secondUser.Photo,
                        Content = conversation.LastMessage == null ? null : conversation.LastMessage.Content,
                        SecondUserId = secondUserId,
                        IsBlocked = user.BlockedUsers.Any(
                        bl => bl.UserToBlockId == secondUserId) ? true : false
                    });
                }
                else
                {
                    res.Add(new GetConversationDto()
                    {
                        Id=conversation.Id,
                        Photo=conversation.ConversationInfo.PhotoName,
                        Content= conversation.LastMessage == null ? null : conversation.LastMessage.Content,
                    });
                }
            }

            return res;
        }

        public async Task CreateGroupAsync(AddGroupRequest request)
        {
            var user = await _auth.FindByIdUserAsync(request.UserId);

            if (user == null)
                throw new UserNotExistException("Given user not exist!!", 400);

            request.UsersId.Add(user.Id);

            var conversationInfo = new ConversationInfo
            {
                AdminId=user.Id,
                PhotoName= _config.GetValue<string>("defaultgroup"),
                GroupName=request.GroupName
            };

            var conversation = new Conversation
            {
                Type=request.IsChannel==true?ConversationType.Channel:ConversationType.Group,
                
                ConversationInfo=conversationInfo
            };

            await _unit.ConversationRepository.CreateAsync(conversation);

            await _unit.ConversationInfoRepository.CreateAsync(conversationInfo);

            foreach(var id in request.UsersId)
            {
                if((await _auth.FindByIdUserAsync(id)) != null)
                {
                    await _unit.UserConversationRepository.CreateAsync(new UserConversation
                            {
                                UserId = id,
                                Conversation = conversation
                            });
                }
            }

            await _unit.Commit();
        }

        public async Task ChangePhotoAsync(AddPhotoDto model)
        {
            if (model.ConversationId == null)
                throw new ChatNotExistException("Given id is null", 400);

            var conversation = await _unit.ConversationRepository.GetChatContentAsync((int)model.ConversationId);

            if(conversation==null||conversation.ConversationInfo.AdminId!=model.UserId)
                throw new ChatNotExistException("Conversation photo cannot be changed!!", 400);

            conversation.ConversationInfo.PhotoName = await this._photoHelper.SavePhotoAsync(model);

            await _unit.Commit();
        }
    }
}