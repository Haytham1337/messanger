using Application.Models.ChatDto.Requests;
using Application.Models.UserDto;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Domain;
using Domain.Entities;
using Domain.Exceptions.ChatExceptions;
using Infrastructure.Services;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ApplicationTests.MessageServiceTests
{
    public class GetMessageByChatTests
    {
        [Fact]
        public async void GetMessageByChat_ChatNotExist_ThrowsException()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mockAuth = fixture.Freeze<Mock<IUnitOfWork>>();
            mockAuth.Setup(a => a.ConversationRepository.GetChatContentAsync(It.IsAny<int>()))
                .ReturnsAsync(default(Conversation));

            var messageService = fixture.Create<MessageService>();

            //assert
            await Assert.ThrowsAsync<ConversationNotExistException>
                (async () => await messageService.GetMessageByChatAsync(new GetChatMessagesRequest()));
        }
    }
}
