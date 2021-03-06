using Application;
using Application.IServices;
using Application.Models.MessageDto;
using Domain;
using Infrastructure.Cache;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MessengerAPI.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class Chat : Hub
    {
        private readonly IMessageService _messageService;

        private readonly IUnitOfWork _unit;

        private readonly IUserService _userService;

        private readonly ICache _cache;

        private readonly IOptions<CacheOptions> _cacheOptions;

        public Chat(ICache cache, IMessageService messageService, IUnitOfWork unit,
            IUserService userService, IOptions<CacheOptions> cacheOptions)
        {
            _messageService = messageService;

            _unit = unit;

            _userService = userService;

            _cache = cache;

            _cacheOptions = cacheOptions;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var userChats = await this._unit.ConversationRepository.GetUserChatsAsync(int.Parse(userId));

            userChats.ForEach(async chat =>
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, chat.Id.ToString());
            });

            await base.OnConnectedAsync();
        }

        public async Task SendToAll(AddMessageDto message)
        {
            message.userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var isBlocked = _cache.Get($"{message.userId}:{message.chatId}");

            if (isBlocked == null)
            {
                isBlocked = await this._userService.CheckStatusAsync(message);

                _cache.Set($"{message.userId}:{message.chatId}", isBlocked, TimeSpan.FromSeconds(_cacheOptions.Value.isBlockeTime));
            }

            if (!(bool)isBlocked)
            {
                var newMessage = await _messageService.AddMessageAsync(message);

                await Clients.Group($"{message.chatId}").SendAsync("update", newMessage, message.chatId);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var userChats = await this._unit.ConversationRepository.GetUserChatsAsync(int.Parse(userId));

            userChats.ForEach(async chat =>
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, chat.Id.ToString());
            });

            await base.OnDisconnectedAsync(exception);
        }
    }
}
