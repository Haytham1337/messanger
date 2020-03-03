using Application.Models.ChatDto.Requests;
using AutoFixture;
using AutoFixture.AutoMoq;
using Domain;
using Domain.Entities;
using Domain.Exceptions.UserExceptions;
using Infrastructure.Services;
using Moq;
using Xunit;

namespace ApplicationTests.ChatServiceTests
{
    public class GetChatsTests
    {
        [Fact]
        public async void GetChats_UserNotExist_ThrowsException()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mockUnit = fixture.Freeze<Mock<IUnitOfWork>>();
            mockUnit.Setup(a => a.UserRepository.GetUserWithBlackList(It.IsAny<string>()))
                .ReturnsAsync(default(User));

            var chatService = fixture.Create<ConversationService>();

            //assert
            await Assert.ThrowsAsync<UserNotExistException>
                (async () => await chatService.GetChatsAsync(new GetChatsRequestDto()));
        }

        [Fact]
        public async void GetChats_GetUserChats_InvokesOnce()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mockUnit = fixture.Freeze<Mock<IUnitOfWork>>();

            var chatService = fixture.Create<ConversationService>();

            //act
            await chatService.GetChatsAsync(new GetChatsRequestDto());

            //assert
            mockUnit.Verify(u => u.ConversationRepository.GetUserChatsAsync(It.IsAny<int>()), Times.Once);
        }
    }
}
