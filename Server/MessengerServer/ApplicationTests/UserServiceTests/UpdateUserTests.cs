//using Application.Models.UserDto;
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

//namespace ApplicationTests.UserServiceTests
//{
//   public class UpdateUserTests
//    {
//        [Fact]
//        public async void UpdateUser_UserNotExist_ThrowsException()
//        {
//            //arrange
//            var fixture = new Fixture().Customize(new AutoMoqCustomization());
//            var mock = fixture.Freeze<Mock<IAuthService>>();

//            mock.Setup(u => u.FindByIdUserAsync(It.IsAny<int>()))
//                    .ReturnsAsync(default(User));

//            var userService = fixture.Create<UserService>();

//            //assert
//            await Assert.ThrowsAsync<UserNotExistException>(async () => await userService.UpdateUserAsync(new UpdateUserDto()));
//        }

//        [Fact]
//        public async void UpdateUser_SaveChanges_InvokesOnce()
//        {
//            //arrange
//            var mockUnit = new Mock<IUnitOfWork>();

//            var mockAuth = new Mock<IAuthService>();
//            mockAuth.Setup(a => a.FindByIdUserAsync(It.IsAny<int>()))
//                .ReturnsAsync(new User());

//            var userService = new UserService(mockUnit.Object, null, mockAuth.Object);

//            //act
//            await userService.UpdateUserAsync(new UpdateUserDto());

//            //assert
//            mockUnit.Verify(u => u.Commit(), Times.Once);
//        }

//        [Fact]
//        public async void UpdateUser_UserExist_ValuesUpdates()
//        {
//            //arrange
//            var user = new User
//            {
//                Age = 25
//            };

//            var fixture = new Fixture().Customize(new AutoMoqCustomization());
//            var mockAuth = fixture.Freeze<Mock<IAuthService>>();

//            mockAuth.Setup(a => a.FindByIdUserAsync(It.IsAny<int>()))
//                .ReturnsAsync(user);

//            var userService = fixture.Create<UserService>();

//            //act
//            await userService.UpdateUserAsync(new UpdateUserDto()
//            {
//                UserId = 10,
//                Age = 20
//            });

//            //assert
//            Assert.Equal<int>(20,user.Age);
//        }
//    }
//}
