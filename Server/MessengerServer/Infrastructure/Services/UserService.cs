﻿using Application.IServices;
using Application.Models.MessageDto;
using Application.Models.PhotoDto;
using Application.Models.UserDto;
using Application.Models.UserDto.Requests;
using AutoMapper;
using Domain;
using Domain.Entities;
using Domain.Exceptions.BlockedUserExceptions;
using Domain.Exceptions.ChatExceptions;
using Domain.Exceptions.UserExceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class UserService:IUserService
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _map;
        private readonly IPhotoHelper _photoHelper;
        private readonly IAuthService _auth;

        public UserService(IUnitOfWork unit,IMapper map,IAuthService auth,IPhotoHelper photoHelper)
        {
            _unit = unit;

            _auth = auth;

            _map = map;

            _photoHelper = photoHelper;
        }

        public async Task<GetUserDto> GetUserInfoAsync(GetUserInfoRequest request)
        {
            var user= await _unit.UserRepository.GetWithPhotoAsync(request.UserName);

            if (user == null)
                throw new UserNotExistException("Given user not exist!!",400);
            
            return _map.Map<GetUserDto>(user);
        }

        public async Task UpdateUserAsync(UpdateUserDto model)
        {
            var user = await _auth.FindByIdUserAsync(model.UserId);

            if (user == null)
                throw new UserNotExistException("Given user not exist!!", 400);
      
            user.Age = model.Age;

            user.PhoneNumber = model.Phone;

            user.NickName = model.NickName;

            await _unit.Commit();         
        }

        public  async Task<List<SearchUserDto>> SearchUserAsync(SearchRequest request)
        {
            var currentUser = await _auth.FindByIdUserAsync(request.UserId);

            if (currentUser == null)
                throw new UserNotExistException("Given user not exist!!", 400);

            var users = (await _unit.UserRepository.SearchUsersAsync(request.Filter));

            users.Remove(currentUser);

            var res = _map.Map<List<SearchUserDto>>(users);

            return res;
        }

        public async Task BlockUserAsync(BlockUserRequest request) 
        {
            var currentUser = await this._auth.FindByIdUserAsync(request.UserId);

            if (currentUser == null)
                throw new UserNotExistException("Given user not exist!!",400);
            
            var userToBlock = await this._unit.UserRepository.GetAsync(request.UserIdToBlock);

            if (userToBlock == null)
                throw new UserNotExistException("User to block not exist!!", 400);


            var blockedUser = await this._unit.BlockedUserRepository
                              .IsBlockedUserAsync(currentUser.Id, request.UserIdToBlock);

            if (blockedUser != null)
                throw new BlockedUserAlreadyExistException("Given user to block is already blocked!!",400);

            
            var newBlockedUser = new BlockedUser()
            {
                UserId = currentUser.Id,
                UserToBlockId = request.UserIdToBlock
            };

            await this._unit.BlockedUserRepository
                    .CreateAsync(newBlockedUser);

            await this._unit.Commit();
        }

        public async Task UnBlockUserAsync(BlockUserRequest request)
        {
            var currentUser = await this._auth.FindByIdUserAsync(request.UserId);

            if (currentUser == null)
                throw new UserNotExistException("Given user not exist!!", 400);

            var blockedUser = await this._unit.BlockedUserRepository
                              .IsBlockedUserAsync(currentUser.Id, request.UserIdToBlock);

            if (blockedUser == null)
                throw new BlockedUserNotExistException("User to unblock not exist!!", 400);

            
            await this._unit.BlockedUserRepository.DeleteAsync(blockedUser.Id);

            await this._unit.Commit();
        }

        public async Task<bool> CheckStatusAsync(AddMessageDto request)
        {
            var chat = await this._unit.ConversationRepository.GetWithUsersConversationsAsync(request.chatId);

            if (chat == null)
                throw new ConversationNotExistException("Given chat not exist!!", 400);

            if (chat.Type != ConversationType.Chat)
                return false;

            var currentUser = await this._auth.FindByIdUserAsync(request.userId);

            if (currentUser == null)
                throw new UserNotExistException("Given user not exist!!", 400);

            var requestedUserId = chat.UserConversations[0].UserId == currentUser.Id ? chat.UserConversations[1].UserId : chat.UserConversations[0].UserId;

            if ((await _unit.BlockedUserRepository.IsBlockedUserAsync(requestedUserId, currentUser.Id)) == null)
            {
                return false;
            }

            return true;
        }

        public async Task ChangePhotoAsync(AddPhotoDto model)
        {
            var user = await _auth.FindByIdUserAsync(model.UserId);

            if (user == null)
                throw new UserNotExistException("Given user not exist!!", 400);

            user.Photo = await this._photoHelper.SavePhotoAsync(model);

            await _unit.Commit();
        }
    }
}
