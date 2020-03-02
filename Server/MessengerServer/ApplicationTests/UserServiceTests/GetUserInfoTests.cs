//using System;
//using System.Collections.Generic;
//using System.Text;
//using Xunit;
//using Moq;
//using Domain;
//using Domain.Entities;
//using Autofac.Extras.Moq;
//using Infrastructure.Services;
//using Application.Models.UserDto;
//using Domain.Exceptions.UserExceptions;
//using AutoMapper;
//using AutoFixture;
//using AutoFixture.AutoMoq;

//namespace ApplicationTests.UserServiceTests
//{
//    public class GetUserInfoTests
//    {
//        [Fact]
//        public async void GetUserInfo_UserNotExist_ThrowsException()
//        {
//            //arrange
//            var mock = new Mock<IUnitOfWork>();          
//            mock.Setup(u => u.UserRepository.GetWithPhotoAsync(It.IsAny<string>()))
//                    .ReturnsAsync(default(User));

//            var userService = new UserService(mock.Object,null,null);

//            var request = new GetUserInfoRequest()
//            {
//                UserName = "UserName"
//            };

//            //assert
//            await Assert.ThrowsAsync<UserNotExistException>(async () => await userService.GetUserInfoAsync(request));
//        }

//        [Fact]
//        public async void GetUserInfo_GetWithPhotoAsync_InvokesOnce()
//        {
//            //arrange
//            var fixture = new Fixture().Customize(new AutoMoqCustomization());
//            var mock=fixture.Freeze<Mock<IUnitOfWork>>();

//            var user = new User();
//            mock.Setup(u => u.UserRepository.GetWithPhotoAsync(It.IsAny<string>()))
//                    .ReturnsAsync(user);

//            var userService = fixture.Create<UserService>();

//            var request = new GetUserInfoRequest()
//            {
//                UserName = "UserName"
//            };

//            //act
//            await  userService.GetUserInfoAsync(request);

//            //assert
//            mock.Verify(u => u.UserRepository.GetWithPhotoAsync(It.IsAny<string>()), Times.Once);
//        }

//        [Fact]
//        public async void GetUserInfo_UserExist_ValidOutput()
//        {
//            //arrange

//            var user = new User() 
//            {
//                NickName="TestUser"
//            };

//            var mockUnit = new Mock<IUnitOfWork>();
//            mockUnit.Setup(u => u.UserRepository.GetWithPhotoAsync(It.IsNotNull<string>()))
//                    .ReturnsAsync(user);

//            var mockMapper = new Mock<IMapper>();
//            mockMapper.Setup(m => m.Map<GetUserDto>(It.IsIn(user)))
//                .Returns(new GetUserDto() { NickName=user.NickName });

//            var userService = new UserService(mockUnit.Object, mockMapper.Object, null);

//            var request = new GetUserInfoRequest() { UserName = "TestUserName" };

//            //act
//            var result = await userService.GetUserInfoAsync(request);

//            //assert
//            Assert.True(result.NickName==user.NickName);
//        }
//    }
//}
