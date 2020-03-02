using Application.IServices;
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
        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _env;
        private readonly IAuthService _auth;

        public UserService(IUnitOfWork unit,IMapper map,IAuthService auth,IConfiguration config,
            IHostingEnvironment env)
        {
            _unit = unit;

            _auth = auth;

            _map = map;

            _config = config;

            _env = env;

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

        public  async Task<List<SearchUserDto>> SearchUserAsync(SearchUserDtoRequest request)
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
                throw new ChatNotExistException("Given chat not exist!!", 400);

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

            var ext = model.UploadedFile.FileName.Substring(model.UploadedFile.FileName.LastIndexOf('.'));

            if (this._config[$"PhotoExtensions:{ext}"] != null)
            {
                user.Photo = $"{user.Id}{model.UploadedFile.Name}";

                var path = $"{_env.WebRootPath}\\avatars\\{user.Photo}";

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await model.UploadedFile.CopyToAsync(fileStream);
                }

                await _unit.Commit();
            }
            else
            {
                throw new PhotoInCorrectException("Given extension is incorrect!!", 400);
            }
        }
    }
}
