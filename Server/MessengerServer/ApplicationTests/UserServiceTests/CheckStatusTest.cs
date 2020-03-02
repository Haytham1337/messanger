using Application.Models.MessageDto;
using AutoFixture;
using AutoFixture.AutoMoq;
using Domain;
using Domain.Entities;
using Domain.Exceptions.ChatExceptions;
using Domain.Exceptions.UserExceptions;
using Infrastructure.Services;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ApplicationTests.UserServiceTests
{
    public class CheckStatusTest
    {
        [Fact]
        public async void CheckStatus_ChatNotExist_ThrowsException()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mockUnit = fixture.Freeze<Mock<IUnitOfWork>>();
            mockUnit.Setup(u => u.ConversationRepository.GetWithUsersConversationsAsync(It.IsAny<int>()))
                .ReturnsAsync(default(Conversation));

            var userService = fixture.Create<UserService>();

            //assert
            await Assert.ThrowsAsync<ChatNotExistException>
                (async () => await userService.CheckStatusAsync(fixture.Create<AddMessageDto>()));
        }

        [Fact]
        public async void CheckStatus_UserNotExist_ThrowsException()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mockUnit = fixture.Freeze<Mock<IAuthService>>();
            mockUnit.Setup(u => u.FindByIdUserAsync(It.IsAny<int>()))
                .ReturnsAsync(default(User));

            var userService = fixture.Create<UserService>();

            //assert
            await Assert.ThrowsAsync<UserNotExistException>
                (async () => await userService.CheckStatusAsync(new AddMessageDto()));
        }

        [Fact]
        public async void CheckStatus_UserBlocked_ReturnsFalse()
        {
            //arrange

            var conversation = new Conversation
            {
                UserConversations = new List<UserConversation>
                {
                    new UserConversation
                    {
                        UserId=1
                    },
                    new UserConversation
                    {
                        UserId=2
                    }
                }
            };

            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mockUnit = fixture.Freeze<Mock<IUnitOfWork>>();
            mockUnit.Setup(u => u.BlockedUserRepository.IsBlockedUserAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new BlockedUser());

            mockUnit.Setup(u => u.ConversationRepository.GetWithUsersConversationsAsync(It.IsAny<int>()))
               .ReturnsAsync(conversation);


            var userService = fixture.Create<UserService>();

            //act
            var result =await userService.CheckStatusAsync(new AddMessageDto());

            //assert
            Assert.True(result);
        }

        [Fact]
        public async void CheckStatus_UserNotBlocked_ReturnsTrue()
        {
            //arrange
            var conversation = new Conversation
            {
                UserConversations = new List<UserConversation>
                {
                    new UserConversation
                    {
                        UserId=1
                    },
                    new UserConversation
                    {
                        UserId=2
                    }
                }
            };


            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mockUnit = fixture.Freeze<Mock<IUnitOfWork>>();
            mockUnit.Setup(u => u.BlockedUserRepository.IsBlockedUserAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(default(BlockedUser));

            mockUnit.Setup(u => u.ConversationRepository.GetWithUsersConversationsAsync(It.IsAny<int>()))
              .ReturnsAsync(conversation);


            var userService = fixture.Create<UserService>();

            //act
            var result = await userService.CheckStatusAsync(new AddMessageDto());


            //assert
            Assert.False(result);
        }
    }
}
