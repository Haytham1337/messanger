//using Application.Models.ChatDto.Requests;
//using AutoFixture;
//using AutoFixture.AutoMoq;
//using Domain;
//using Domain.Entities;
//using Domain.Exceptions.UserExceptions;
//using Infrastructure.Services;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using Xunit;

//namespace ApplicationTests.ChatServiceTests
//{
//    public class GetChatsTests
//    {
//        [Fact]
//        public async void GetChats_UserNotExist_ThrowsException()
//        {
//            //arrange
//            var fixture = new Fixture().Customize(new AutoMoqCustomization());

//            var mockUnit = fixture.Freeze<Mock<IUnitOfWork>>();
//            mockUnit.Setup(a => a.UserRepository.GetUserWithBlackList(It.IsAny<string>()))
//                .ReturnsAsync(default(User));

//            var chatService = fixture.Create<ConversationService>();

//            //assert
//            await Assert.ThrowsAsync<UserNotExistException>
//                (async () => await chatService.GetChatsAsync(new GetChatsRequestDto()));
//        }

//        [Fact]
//        public async void GetChats_GetUserChats_InvokesOnce()
//        {
//            //arrange
//            var fixture = new Fixture().Customize(new AutoMoqCustomization());

//            var mockUnit = fixture.Freeze<Mock<IUnitOfWork>>();

//            var chatService = fixture.Create<ConversationService>();

//            //act
//            await chatService.GetChatsAsync(new GetChatsRequestDto());

//            //assert
//            mockUnit.Verify(u => u.ConversationRepository.GetUserChatsAsync(It.IsAny<int>()), Times.Once);
//        }

//        [Fact]
//        public async void GetChats_ReturnsValidValues()
//        {
//            //arrange
//            var chats = new List<Conversation>
//            {
//                new Conversation
//                {
//                    Id=1,
//                    SecondUser=new User
//                    {
//                        Id=2,
//                        Photo=new Photo
//                        {
//                            Name="TestPhoto"
//                        }
//                    },
//                    FirstUserId=3
//                }
//            };

//            var user = new User
//            {
//                Id = 3
//            };

//            var fixture = new Fixture().Customize(new AutoMoqCustomization());

//            var mockUnit = fixture.Freeze<Mock<IUnitOfWork>>();
//            mockUnit.Setup(u => u.UserRepository.GetUserWithBlackList(It.IsAny<string>()))
//                .ReturnsAsync(user);
//            mockUnit.Setup(u => u.ConversationRepository.GetUserChatsAsync(It.IsAny<int>()))
//                .ReturnsAsync(chats);

//            var chatService = fixture.Create<ConversationService>();

//            //act
//            var result = await chatService.GetChatsAsync(new GetChatsRequestDto());

//            //assert
//            Assert.True(result[0].Photo == chats[0].SecondUser.Photo.Name);
//        }
//    }
//}
