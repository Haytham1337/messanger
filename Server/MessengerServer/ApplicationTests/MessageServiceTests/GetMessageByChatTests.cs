//using Application.Models.ChatDto.Requests;
//using Application.Models.UserDto;
//using AutoFixture;
//using AutoFixture.AutoMoq;
//using AutoMapper;
//using Domain;
//using Domain.Entities;
//using Domain.Exceptions.ChatExceptions;
//using Infrastructure.Services;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using Xunit;

//namespace ApplicationTests.MessageServiceTests
//{
//    public class GetMessageByChatTests
//    {
//        [Fact]
//        public async void GetMessageByChat_ChatNotExist_ThrowsException()
//        {
//            //arrange
//            var fixture = new Fixture().Customize(new AutoMoqCustomization());

//            var mockAuth = fixture.Freeze<Mock<IUnitOfWork>>();
//            mockAuth.Setup(a => a.ConversationRepository.GetChatContentAsync(It.IsAny<int>()))
//                .ReturnsAsync(default(Conversation));

//            var messageService = fixture.Create<MessageService>();

//            //assert
//            await Assert.ThrowsAsync<ChatNotExistException>
//                (async () => await messageService.GetMessageByChatAsync(new GetChatMessagesRequest()));
//        }

//        [Fact]
//        public async void GetMessageByChat_ReturnsValidValues()
//        {
//            //arrange
//            var chat = new Conversation
//            {
//                FirstUser = new User
//                {
//                    NickName = "TestUser"
//                }
//            };

//            var fixture = new Fixture().Customize(new AutoMoqCustomization());

//            var mockAuth = fixture.Freeze<Mock<IUnitOfWork>>();
//            mockAuth.Setup(a => a.ConversationRepository.GetChatContentAsync(It.IsAny<int>()))
//                .ReturnsAsync(chat);

//            var mockMap = fixture.Freeze<Mock<IMapper>>();
//            mockMap.Setup(a => a.Map<List<GetUserDto>>(It.Is<List<User>>(l=>l[0].NickName=="TestUser")))
//                .Returns(new List<GetUserDto>() { new GetUserDto() {NickName="TestUser"}});

//            var messageService = fixture.Create<MessageService>();

//            //act
//            var result = await messageService.GetMessageByChatAsync(new GetChatMessagesRequest());

//            //assert
//            Assert.True(chat.FirstUser.NickName == result.Users[0].NickName);
//        }
//    }
//}
