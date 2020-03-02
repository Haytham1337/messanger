using Application.Models.MessageDto;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Domain;
using Domain.Entities;
using Domain.Exceptions.ChatExceptions;
using Domain.Exceptions.MessageExceptions;
using Domain.Exceptions.UserExceptions;
using Infrastructure.Services;
using Moq;
using Xunit;

namespace ApplicationTests.MessageServiceTests
{
    public class AddMessageTests
    {

        [Fact]
        public async void AddMessage_UserNotExist_ThrowsException()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mockAuth = fixture.Freeze<Mock<IAuthService>>();
            mockAuth.Setup(a => a.FindByIdUserAsync(It.IsAny<int>()))
                .ReturnsAsync(default(User));

            var messageService = fixture.Create<MessageService>();

            //assert
            await Assert.ThrowsAsync<UserNotExistException>
                (async () => await messageService.AddMessageAsync(new AddMessageDto()));
        }


        [Fact]
        public async void AddMessage_ChatNotExist_ThrowsException()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mockAuth = fixture.Freeze<Mock<IUnitOfWork>>();
            mockAuth.Setup(a => a.ConversationRepository.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(default(Conversation));

            var messageService = fixture.Create<MessageService>();

            //assert
            await Assert.ThrowsAsync<ChatNotExistException>
                (async () => await messageService.AddMessageAsync(new AddMessageDto()));
        }

        [Fact]
        public async void AddMessage_MessageIsEmpty_ThrowsException()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var messageService = fixture.Create<MessageService>();

            //assert
            await Assert.ThrowsAsync<MessageInCorrectException>
                (async () => await messageService.AddMessageAsync(new AddMessageDto()));
        }

        [Fact]
        public async void AddMessage_CreateMessage_InvokesOnce()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mockUnit = fixture.Freeze<Mock<IUnitOfWork>>();
          
            var messageService = fixture.Create<MessageService>();

            //act
            await messageService.AddMessageAsync(new AddMessageDto { Content = "content" });

            //assert
            mockUnit.Verify(u => u.MessageRepository.CreateAsync(It.IsAny<Message>()),Times.Once);
        }

        [Fact]
        public async void AddMessage_ReturnsValidValues()
        {
            //arrange
            var getMessage = new GetMessageDto()
            {
                Content = "TestContent"
            };

            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mockMap = fixture.Freeze<Mock<IMapper>>();
            mockMap.Setup(m => m.Map<GetMessageDto>(It.Is<Message>(m => m.Content == getMessage.Content)))
                .Returns(getMessage);

            var messageService = fixture.Create<MessageService>();

            //act
            var result=await messageService.AddMessageAsync(new AddMessageDto { Content = "TestContent" });

            //assert
            Assert.True(result.Content == getMessage.Content);
        }
    }
}
