using Application.Models.UserDto;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Domain;
using Domain.Entities;
using Domain.Exceptions.UserExceptions;
using Infrastructure.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ApplicationTests.UserServiceTests
{
    public class SearchUserTests
    {
        [Fact]
        public async void SearchUser_UserNotExist_ThrowsException()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var mockAuth = fixture.Freeze<Mock<IAuthService>>();

            mockAuth.Setup(a => a.FindByIdUserAsync(It.IsAny<int>()))
                    .ReturnsAsync(default(User));

            var userService = fixture.Create<UserService>();

            //assert
            await Assert.ThrowsAsync<UserNotExistException>(async () => await userService.SearchUserAsync(new SearchRequest()));
        }

        [Fact]
        public async void SearchUser_UserExist_RemovesRequestedUser()
        {
            //arrange
            var user = new User
            {
                Email = "TestUserEmail"
            };

            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            var mockAuth = fixture.Freeze<Mock<IAuthService>>();
            mockAuth.Setup(a => a.FindByIdUserAsync(It.IsAny<int>()))
                    .ReturnsAsync(user);

            var usersSearchResult = new List<User>
            {
                user,
                new User
                {
                    Email="SecondTestUserEmail"
                }
            };

            var mockUnit = fixture.Freeze<Mock<IUnitOfWork>>();
            mockUnit.Setup(u => u.UserRepository.SearchUsersAsync(It.IsAny<string>()))
                .ReturnsAsync(usersSearchResult);

            var userService = fixture.Create<UserService>();

            //act
            await userService.SearchUserAsync(new SearchRequest());

            //assert
            Assert.DoesNotContain<User>(user,usersSearchResult);
        }

        [Fact]
        public async void SearchUser_ReturnsUsersCorrectly()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var searchResult = new List<SearchUserDto>
            {
                new SearchUserDto
                {
                    Email="TestEmail"
                }
            };

            var mockUnit = fixture.Freeze<Mock<IMapper>>();
            mockUnit.Setup(u => u.Map<List<SearchUserDto>>(It.IsAny<List<User>>()))
                .Returns(searchResult);

            var userService = fixture.Create<UserService>();

            //act
            var res=await userService.SearchUserAsync(new SearchRequest());

            //assert
            Assert.True(res.Count==searchResult.Count);
        }

        [Fact]
        public async void SearchUser_SearchMethod_InvokesOnce()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mockUnit = fixture.Freeze<Mock<IUnitOfWork>>();

            var userService = fixture.Create<UserService>();

            //act
            await userService.SearchUserAsync(new SearchRequest());

            //assert
            mockUnit.Verify(u => u.UserRepository.SearchUsersAsync(It.IsAny<string>()), Times.Once);
        }
    }
}
