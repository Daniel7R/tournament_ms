using System.Net;
using Moq;
using Moq.Protected;
using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.Services;
using Xunit;

namespace TournamentMS.Tests
{
    public class UserServiceTests
    {

        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://usersapi.com") 
            };

            _mockLogger = new Mock<ILogger<UserService>>();
            _userService = new UserService(_httpClient, _mockLogger.Object);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenRequestIsSuccessful()
        {
            // Arrange
            var userId = 1;
            var expectedUser = new UserDTO { IdUser = userId, Name = "Test User" };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(expectedUser)
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.Equal(expectedUser.IdUser, result.IdUser);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            Assert.Equal(expectedUser.Name, result.Name);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenRequestFails()
        {
            // Arrange
            var userId = 1;

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.Null(result);
        }
    }
}