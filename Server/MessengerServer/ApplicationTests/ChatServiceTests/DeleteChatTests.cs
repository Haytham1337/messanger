using Application.Models.ConversationDto.Requests;
using AutoFixture;
using AutoFixture.AutoMoq;
using Domain;
using Domain.Entities;
using Domain.Exceptions.ChatExceptions;
using Domain.Exceptions.UserExceptions;
using Infrastructure.Services;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationTests.ChatServiceTests
{
    [TestFixture]
    public class DeleteChatTests
    {
        IFixture _fixture;
        Mock<IUnitOfWork> unitOfWorkMock;

        [OneTimeSetUp]
        public void Init()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        }

        [Test]
        public void DeleteChat_ChatNotExists_ThrowsException()
        {
            //arrange
            unitOfWorkMock.Setup(unit => unit.ConversationRepository.GetWithUsersConversationsAsync(It.IsAny<int>()))
                .ReturnsAsync(default(Conversation));

            var chatService = _fixture.Create<ConversationService>();

            //assert
            Assert.ThrowsAsync<ConversationNotExistException>(
                () => chatService.DeleteConversationAsync(_fixture.Create<DeleteRequest>()));
        }

        [Test]
        public async Task DeleteChat_DeleteAsync_InvokesOnce()
        {
            //arrange
            var conversation = new Conversation
            {
                Id = 2,
                Type = ConversationType.Chat,
                UserConversations = new List<UserConversation>
                {
                    new UserConversation
                    {
                        UserId = 1
                    }
                }
            };

            unitOfWorkMock.Setup(unit => unit.ConversationRepository.GetWithUsersConversationsAsync(It.IsAny<int>()))
                .ReturnsAsync(conversation);

            var conversationService = _fixture.Create<ConversationService>();

            //act
            await conversationService.DeleteConversationAsync(new DeleteRequest() { UserId = 1 });

            //assert
            unitOfWorkMock.Verify(unit => unit.ConversationRepository.DeleteAsync(2), Times.Once);
        }

        [Test]
        public void DeleteChat_UserNotChatMember_ThrowsException()
        {
            //arrange
            var conversation = new Conversation
            {
                Id = 2,
                Type = ConversationType.Chat,
                UserConversations = new List<UserConversation>()
            };

            unitOfWorkMock.Setup(unit => unit.ConversationRepository.GetWithUsersConversationsAsync(It.IsAny<int>()))
                .ReturnsAsync(conversation);

            var conversationService = _fixture.Create<ConversationService>();

            //act
            var result = conversationService.DeleteConversationAsync(new DeleteRequest() { UserId = 1 });

            //assert
            Assert.ThrowsAsync<UserNotHaveRigthsException>(()=>result);
        }

        [Test]
        public void DeleteChat_UserNotAdmin_ThrowsException()
        {
            //arrange
            var conversation = new Conversation
            {
                Id = 2,
                Type = ConversationType.Channel,
                ConversationInfo = new ConversationInfo { AdminId = 2},
                UserConversations = new List<UserConversation>
                {
                    new UserConversation
                    {
                        UserId = 1
                    }
                }
            };

            unitOfWorkMock.Setup(unit => unit.ConversationRepository.GetWithUsersConversationsAsync(It.IsAny<int>()))
                .ReturnsAsync(conversation);

            var conversationService = _fixture.Create<ConversationService>();

            //act
            var result = conversationService.DeleteConversationAsync(new DeleteRequest() { UserId = 1 });

            //assert
            Assert.ThrowsAsync<UserNotHaveRigthsException>(() => result);
        }
    }
}
