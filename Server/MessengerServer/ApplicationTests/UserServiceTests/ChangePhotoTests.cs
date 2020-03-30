using Application.Models.PhotoDto;
using AutoFixture;
using AutoFixture.AutoMoq;
using Domain.Entities;
using Domain.Exceptions.UserExceptions;
using Infrastructure.Services;
using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace ApplicationTests.UserServiceTests
{
    public class ChangePhotoTests
    {
        [Fact]
        public async void ChangePhoto_UserNotExist_ThrowsException()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mockAuth = fixture.Freeze<Mock<IAuthService>>();
            mockAuth.Setup(a => a.FindByIdUserAsync(It.IsAny<int>()))
                .ReturnsAsync(default(User));

            var userService = fixture.Create<UserService>();

            //assert
            await Assert.ThrowsAsync<UserNotExistException>
                (async () => await userService.ChangePhotoAsync(new AddPhotoDto()));
        }

        [Fact]
        [System.Obsolete]
        public async void ChangePhoto_ExtensionNotExist_ThrowsException()
        {
            //arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mockConfig = fixture.Freeze<Mock<IConfiguration>>();
            mockConfig.SetupGet(c => c[It.IsAny<string>()])
                .Returns(default(string));

            var fileMock = new Mock<IFormFile>();
            fileMock.SetupGet(file => file.FileName)
                .Returns("photo.extension");

            var photoHelper = fixture.Create<PhotoHelper>();

            //assert
            await Assert.ThrowsAsync<PhotoInCorrectException>(async () => await photoHelper.SavePhotoAsync(fileMock.Object));
        }
    }
}
