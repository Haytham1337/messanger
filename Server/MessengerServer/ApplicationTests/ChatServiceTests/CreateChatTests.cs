

using Application.Models.ChatDto.Requests;
using AutoFixture;
using AutoFixture.AutoMoq;
using Domain;
using Domain.Entities;
using Domain.Exceptions.ChatExceptions;
using Domain.Exceptions.UserExceptions;
using Infrastructure.Services;
using Moq;
using Xunit;

namespace ApplicationTests.ChatServiceTests
{
    public class CreateChatTests
    {
        [Fact]
        public async void CreateChat_UserNotExist_ThrowsException()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mockAuth = fixture.Freeze<Mock<IAuthService>>();
            mockAuth.Setup(a => a.FindByIdUserAsync(It.IsAny<int>()))
                .ReturnsAsync(default(User));

            var chatService = fixture.Create<ConversationService>();

            //assert
            await Assert.ThrowsAsync<UserNotExistException>
                (async () => await chatService.CreateChatAsync(new AddConversationRequest()));
        }

        [Fact]
        public async void CreateChat_ChatExist_ThrowsException()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mockUnit = fixture.Freeze<Mock<IUnitOfWork>>();
            mockUnit.Setup(a => a.ConversationRepository.ChatExistAsync(It.IsAny<int>(),It.IsAny<int>()))
                .ReturnsAsync(false);

            var chatService = fixture.Create<ConversationService>();

            //assert
            await Assert.ThrowsAsync<ConversationAlreadyExistException>
                (async () => await chatService.CreateChatAsync(new AddConversationRequest()));
        }

        [Fact]
        public async void CreateChat_Creat_InvokesOnce()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mockUnit = fixture.Freeze<Mock<IUnitOfWork>>();
            mockUnit.Setup(a => a.ConversationRepository.ChatExistAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            var chatService = fixture.Create<ConversationService>();

            //act
            await chatService.CreateChatAsync(new AddConversationRequest());

            //assert
            mockUnit.Verify(u => u.ConversationRepository.CreateAsync(It.IsAny<Conversation>()), Times.Once);
        }
    }
}
