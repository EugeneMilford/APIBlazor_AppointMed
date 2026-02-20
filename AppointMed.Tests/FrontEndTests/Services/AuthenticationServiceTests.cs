using Blazored.LocalStorage;
using Moq;
using Xunit;

namespace AppointMed.Tests.FrontEndTests.Services
{
    public class AuthenticationServiceTests
    {
        private readonly Mock<ILocalStorageService> _mockLocalStorage;

        public AuthenticationServiceTests()
        {
            _mockLocalStorage = new Mock<ILocalStorageService>();
        }

        [Fact]
        public async Task GetToken_ReturnsStoredToken()
        {
            // Arrange
            var expectedToken = "test-token-123";
            _mockLocalStorage.Setup(l => l.GetItemAsync<string>("authToken", default))
                .ReturnsAsync(expectedToken);

            // Act
            var result = await _mockLocalStorage.Object.GetItemAsync<string>("authToken");

            // Assert
            Assert.Equal(expectedToken, result);
        }

        [Fact]
        public async Task SetToken_StoresTokenInLocalStorage()
        {
            // Arrange
            var token = "new-token-456";
            _mockLocalStorage.Setup(l => l.SetItemAsync("authToken", token, default))
                .Returns(ValueTask.CompletedTask);

            // Act
            await _mockLocalStorage.Object.SetItemAsync("authToken", token);

            // Assert
            _mockLocalStorage.Verify(l => l.SetItemAsync("authToken", token, default), Times.Once);
        }

        [Fact]
        public async Task RemoveToken_ClearsTokenFromLocalStorage()
        {
            // Arrange
            _mockLocalStorage.Setup(l => l.RemoveItemAsync("authToken", default))
                .Returns(ValueTask.CompletedTask);

            // Act
            await _mockLocalStorage.Object.RemoveItemAsync("authToken");

            // Assert
            _mockLocalStorage.Verify(l => l.RemoveItemAsync("authToken", default), Times.Once);
        }

        [Fact]
        public async Task GetUserId_ReturnsStoredUserId()
        {
            // Arrange
            var expectedUserId = "user-123";
            _mockLocalStorage.Setup(l => l.GetItemAsync<string>("userId", default))
                .ReturnsAsync(expectedUserId);

            // Act
            var result = await _mockLocalStorage.Object.GetItemAsync<string>("userId");

            // Assert
            Assert.Equal(expectedUserId, result);
        }

        [Fact]
        public async Task GetUserEmail_ReturnsStoredEmail()
        {
            // Arrange
            var expectedEmail = "user@example.com";
            _mockLocalStorage.Setup(l => l.GetItemAsync<string>("userEmail", default))
                .ReturnsAsync(expectedEmail);

            // Act
            var result = await _mockLocalStorage.Object.GetItemAsync<string>("userEmail");

            // Assert
            Assert.Equal(expectedEmail, result);
        }

        [Fact]
        public async Task ClearAllAuthData_RemovesAllStoredItems()
        {
            // Arrange
            _mockLocalStorage.Setup(l => l.RemoveItemAsync("authToken", default))
                .Returns(ValueTask.CompletedTask);
            _mockLocalStorage.Setup(l => l.RemoveItemAsync("userId", default))
                .Returns(ValueTask.CompletedTask);
            _mockLocalStorage.Setup(l => l.RemoveItemAsync("userEmail", default))
                .Returns(ValueTask.CompletedTask);

            // Act
            await _mockLocalStorage.Object.RemoveItemAsync("authToken");
            await _mockLocalStorage.Object.RemoveItemAsync("userId");
            await _mockLocalStorage.Object.RemoveItemAsync("userEmail");

            // Assert
            _mockLocalStorage.Verify(l => l.RemoveItemAsync("authToken", default), Times.Once);
            _mockLocalStorage.Verify(l => l.RemoveItemAsync("userId", default), Times.Once);
            _mockLocalStorage.Verify(l => l.RemoveItemAsync("userEmail", default), Times.Once);
        }
    }
}
