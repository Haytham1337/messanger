using Application;
using Application.IServices;
using Application.Models.ChatDto.Requests;
using Application.Models.MessageDto;
using Application.Models.MessageDto.Requests;
using Application.Models.UserDto;
using AutoMapper;
using Domain;
using Domain.Entities;
using Domain.Exceptions.ChatExceptions;
using Domain.Exceptions.MessageExceptions;
using Domain.Exceptions.UserExceptions;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unit;

        private readonly IAuthService _auth;

        private readonly IMapper _map;

        private readonly ICache _cache;

        private readonly IPhotoHelper _photoHelper;

        public MessageService(IUnitOfWork unit, IAuthService auth, IMapper map,
            ICache cache, IPhotoHelper photoHelper)
        {
            _unit = unit;

            _auth = auth;

            _map = map;

            _cache = cache;

            _photoHelper = photoHelper;
        }

        public async Task<GetMessageDto> AddMessageAsync(AddMessageDto message)
        {
            var user = await _auth.FindByIdUserAsync(message.userId);

            if (user == null)
                throw new UserNotExistException(ExceptionMessages.UserNotExist, 400);

            var chat = await _unit.ConversationRepository.GetAsync(message.chatId);

            if (chat == null)
                throw new ConversationNotExistException(ExceptionMessages.ConversationNotExist, 400);

            if (!string.IsNullOrEmpty(message.Content) || !string.IsNullOrEmpty(message.photo))
            {
                DateTime timevalue = new DateTime();
                DateTime.TryParse(message.timeCreated, out timevalue);

                var newmessage = new Message()
                {
                    Content = message.Content,
                    photo = message.photo,
                    TimeCreated = timevalue,
                    UserId = user.Id,
                    ChatId = message.chatId
                };

                await this._unit.MessageRepository.CreateAsync(newmessage);

                chat.LastMessage = newmessage;

                await _unit.Commit();

                return _map.Map<GetMessageDto>(newmessage);
            }

            throw new MessageInCorrectException(ExceptionMessages.MessageInCorrect, 400);

        }

        public async Task<AllMessagesDto> GetMessageByChatAsync(GetChatMessagesRequest request)
        {
            var chatContent = await this._unit.ConversationRepository.GetChatContentAsync(request.Id);

            var users = await this._unit.UserConversationRepository.GetUsersByConversationAsync(request.Id);

            var messages = await this._unit.MessageRepository.GetMessagesByChat(request.Id, request.portion);


            if (chatContent == null)
                throw new ConversationNotExistException(ExceptionMessages.ConversationNotExist, 400);

            var result = new AllMessagesDto()
            {
                Users = _map.Map<List<GetUserDto>>(users.Select(u => u.User)),
                Messages = _map.Map<List<GetMessageDto>>(messages),
                Type = chatContent.Type,
                AdminId = chatContent.ConversationInfo == null ? null : (int?)chatContent.ConversationInfo.AdminId,
                Name = chatContent.ConversationInfo == null ? null : chatContent.ConversationInfo.GroupName
            };

            result.Users.ForEach(user =>
            {
                user.isOnline = _cache.Exists($"{user.Id}") ? true : false;
            });

            return result;
        }

        public async Task<string> SaveMessagePhotoAsync(IFormFile uploadedFile)
        {
            return await _photoHelper.SavePhotoAsync(uploadedFile);
        }

        public async Task DeleteMessageAsync(DeleteMessageRequest request)
        {
            var message = await _unit.MessageRepository.GetMessageByIdWithConversationInfo(request.MessageId);

            if (message == null)
                throw new MessageNotExistException(ExceptionMessages.MessageInCorrect, 400);

            var user = await _auth.FindByIdUserAsync(request.UserId);

            var ismember = await _unit.ConversationRepository.isUserConversationMember(message.ChatId.Value, request.UserId);

            if (user == null || !ismember)
                throw new UserNotExistException(ExceptionMessages.UserNotExist, 400);

            if (message.Chat.Type == ConversationType.Chat ||
                message.Chat.Type == ConversationType.Group)
            {
                if (message.UserId == request.UserId)
                {
                    await _photoHelper.DeletePhotoAsync(message.photo);

                    await _unit.MessageRepository.DeleteAsync(message.Id);
                }
                else
                {
                    throw new UserNotHaveRigthsException(ExceptionMessages.NotHaveRigths, 400);
                }
            }
            else
            {
                if (message.Chat.ConversationInfo.AdminId == request.UserId)
                {
                    await _photoHelper.DeletePhotoAsync(message.photo);

                    await _unit.MessageRepository.DeleteAsync(message.Id);
                }
                else
                {
                    throw new UserNotHaveRigthsException(ExceptionMessages.NotHaveRigths, 400);
                }
            }

            await _unit.Commit();
        }
    }
}
