﻿using Application.IServices;
using Application.Models.ChatDto.Requests;
using Application.Models.ChatDto.Responces;
using Domain;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unit;

        private readonly AuthService _auth;
        public ChatService(IUnitOfWork unit, AuthService auth)
        {
            _unit = unit;

            _auth = auth;
        }

        public async Task<bool> CreateChat(AddChatRequest request)
        {
            var user = await _auth.FindByNameUserAsync(request.UserName);

            if ((await this._unit.ChatRepository.ChatExist(user.Id, request.SecondUserId)))
            {
                var chat = new Chat()
                {
                    FirstUserId = user.Id,
                    SecondUserId = request.SecondUserId
                };

                await this._unit.ChatRepository.Create(chat);

                await this._unit.Commit();

                return true;
            }

            return false;
        }

        public async Task<List<GetChatDto>> GetChats(GetChatsRequestDto request)
        {
            var user = await _auth.FindByNameUserAsync(request.UserName);

            var chatres= await _unit.ChatRepository.GetUserChats(user.Id);

            var res = new List<GetChatDto>();

            foreach(var chat in chatres)
            {
                res.Add(new GetChatDto()
                {
                    Id = chat.Id,
                    Photo = chat.FirstUserId == user.Id ? chat.SecondUser.Photo.Name : chat.FirstUser.Photo.Name,
                    Content = chat.LastMessage==null?null:chat.LastMessage.Content
                });
            }

            return res;
        }
        
    }
}
