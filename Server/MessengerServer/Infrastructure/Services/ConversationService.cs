using Application;
using Application.IServices;
using Application.Models.ChatDto.Requests;
using Application.Models.ChatDto.Responces;
using Application.Models.ConversationDto.Requests;
using Application.Models.ConversationDto.Responces;
using Application.Models.PhotoDto;
using Application.Models.UserDto;
using AutoMapper;
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

        private readonly IMapper _map;

        private readonly ICache _cache;

        public ConversationService(IUnitOfWork unit, IAuthService auth, IConfiguration config,
            IPhotoHelper photoHelper, IMapper map, ICache cache)
        {
            _unit = unit;

            _auth = auth;

            _config = config;

            _photoHelper = photoHelper;

            _map = map;

            _cache = cache;
        }

        public async Task CreateChatAsync(AddConversationRequest request)
        {
            var user = await _auth.FindByIdUserAsync(request.userId);

            if (user == null)
                throw new UserNotExistException("user not exist", 400);

            if ((await this._unit.ConversationRepository.ChatExistAsync(user.Id, request.id)))
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
                    UserId = request.id,
                    Conversation = chat
                };

                await this._unit.ConversationRepository.CreateAsync(chat);

                await this._unit.UserConversationRepository.CreateAsync(firstUserConversation);

                await this._unit.UserConversationRepository.CreateAsync(secondUserConversation);

                await this._unit.Commit();
            }
            else
            {
                throw new ConversationAlreadyExistException("chat already exist", 400);
            }
        }

        public async Task<List<GetConversationDto>> GetConversationsAsync(GetChatsRequestDto request)
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
                        Content = conversation.LastMessage?.Content,
                        SecondUserId = secondUserId,
                        isOnline = _cache.Get($"{secondUserId}") == null ? false : true,
                        IsBlocked = user.BlockedUsers.Any(
                        bl => bl.UserToBlockId == secondUserId) ? true : false
                    });
                }
                else
                {
                    res.Add(new GetConversationDto()
                    {
                        Id = conversation.Id,
                        Photo = conversation.ConversationInfo.PhotoName,
                        Content = conversation.LastMessage?.Content
                    });
                }
            }

            return res;
        }

        public async Task ChangePhotoAsync(AddPhotoDto model)
        {
            if (model.ConversationId == null)
                throw new ConversationNotExistException("Given id is null", 400);

            var conversation = await _unit.ConversationRepository.GetChatContentAsync((int)model.ConversationId);

            if (conversation == null || conversation.ConversationInfo.AdminId != model.UserId)
                throw new ConversationNotExistException("Conversation photo cannot be changed!!", 400);

            conversation.ConversationInfo.PhotoName = await this._photoHelper.SavePhotoAsync(model);

            await _unit.Commit();
        }

        public async Task<List<SearchConversationResponce>> SearchConversation(SearchRequest request)
        {
            var user = await _auth.FindByIdUserAsync(request.UserId);

            if (user == null)
                throw new UserNotExistException("Given user not exist!!", 400);

            var responce = new List<SearchConversationResponce>();

            var users = await _unit.UserRepository.SearchUsersAsync(request.Filter);

            users.Remove(user);

            var conversations = await _unit.ConversationRepository.SearchConversationsAsync(request.Filter, request.UserId);

            responce.AddRange(this._map.Map<List<SearchConversationResponce>>(users));

            responce.AddRange(this._map.Map<List<SearchConversationResponce>>(conversations));

            return responce.OrderBy(res => res.Name).Take(5).ToList();
        }

        public async Task DeleteConversationAsync(DeleteRequest request)
        {
            var conversation = await _unit.ConversationRepository.GetWithUsersConversationsAsync(request.ConversationId);

            if (conversation == null)
                throw new ConversationNotExistException("Given id is not set!!", 400);

            if (conversation.Type == ConversationType.Chat)
            {

                if (conversation.UserConversations.Any(uconv => uconv.UserId == request.UserId))
                {
                    await _unit.ConversationRepository.DeleteAsync(conversation.Id);
                }
                else
                {
                    throw new UserNotHaveRigthsException("user is not a member!!", 400);
                }
            }
            else
            {
                if (conversation.ConversationInfo.AdminId == request.UserId)
                {
                    await _unit.ConversationRepository.DeleteAsync(conversation.Id);
                }
                else
                {
                    throw new UserNotHaveRigthsException("user is not an admin!!", 400);
                }
            }

            await _unit.Commit();
        }
    }
}