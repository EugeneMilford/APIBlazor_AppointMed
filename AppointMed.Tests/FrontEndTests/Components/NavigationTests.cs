using Microsoft.AspNetCore.Components.Authorization;
using Moq;
using System.Security.Claims;
using Xunit;
using Assert = Xunit.Assert;

namespace AppointMed.Tests.FrontEndTests.Components
{
    public class NavigationTests
    {
        [Fact]
        public void AuthenticationState_ForUnauthenticatedUser_IsNotAuthenticated()
        {
            // Arrange
            var authState = new AuthenticationState(
                new ClaimsPrincipal(new ClaimsIdentity()));

            // Assert
            Assert.False(authState.User.Identity.IsAuthenticated);
        }

        [Fact]
        public void AuthenticationState_ForAuthenticatedUser_IsAuthenticated()
        {
            // Arrange
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "testuser@example.com"),
                new Claim(ClaimTypes.Role, "User")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var authState = new AuthenticationState(new ClaimsPrincipal(identity));

            // Assert
            Assert.True(authState.User.Identity.IsAuthenticated);
            Assert.Equal("testuser@example.com", authState.User.Identity.Name);
        }

        [Fact]
        public void AuthenticationState_ForAdminUser_HasAdminRole()
        {
            // Arrange
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "admin@example.com"),
                new Claim(ClaimTypes.Role, "Administrator")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            // Assert
            Assert.True(user.IsInRole("Administrator"));
            Assert.False(user.IsInRole("User"));
        }

        [Fact]
        public void AuthenticationState_ForRegularUser_HasUserRole()
        {
            // Arrange
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "user@example.com"),
                new Claim(ClaimTypes.Role, "User")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            // Assert
            Assert.True(user.IsInRole("User"));
            Assert.False(user.IsInRole("Administrator"));
        }

        [Fact]
        public async Task AuthenticationStateProvider_ReturnsAuthenticationState()
        {
            // Arrange
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "test@example.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var expectedState = new AuthenticationState(new ClaimsPrincipal(identity));

            var mockAuthStateProvider = new Mock<AuthenticationStateProvider>();
            mockAuthStateProvider.Setup(x => x.GetAuthenticationStateAsync())
                .ReturnsAsync(expectedState);

            // Act
            var state = await mockAuthStateProvider.Object.GetAuthenticationStateAsync();

            // Assert
            Assert.NotNull(state);
            Assert.True(state.User.Identity.IsAuthenticated);
            Assert.Equal("test@example.com", state.User.Identity.Name);
        }

        [Fact]
        public void ClaimsPrincipal_CanExtractUserId()
        {
            // Arrange
            var userId = "user-123";
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, "test@example.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            // Act
            var extractedUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Assert
            Assert.Equal(userId, extractedUserId);
        }

        [Fact]
        public void ClaimsPrincipal_CanExtractEmail()
        {
            // Arrange
            var email = "test@example.com";
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, email)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            // Act
            var extractedEmail = user.FindFirst(ClaimTypes.Email)?.Value;

            // Assert
            Assert.Equal(email, extractedEmail);
        }

        [Fact]
        public void ClaimsPrincipal_WithMultipleRoles_CanCheckAllRoles()
        {
            // Arrange
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "test@example.com"),
                new Claim(ClaimTypes.Role, "User"),
                new Claim(ClaimTypes.Role, "Manager")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            // Assert
            Assert.True(user.IsInRole("User"));
            Assert.True(user.IsInRole("Manager"));
            Assert.False(user.IsInRole("Administrator"));
        }
    }
}
