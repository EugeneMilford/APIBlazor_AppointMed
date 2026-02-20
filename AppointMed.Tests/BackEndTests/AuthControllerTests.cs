using AppointMed.API.Controllers;
using AppointMed.API.Data;
using AppointMed.API.Models.User;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AppointMed.Tests.BackEndTests
{
    public class AuthControllerTests
    {
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<UserManager<ApiUser>> _mockUserManager;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockLogger = new Mock<ILogger<AuthController>>();
            _mockMapper = new Mock<IMapper>();

            var userStoreMock = new Mock<IUserStore<ApiUser>>();
            _mockUserManager = new Mock<UserManager<ApiUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            _mockConfiguration = new Mock<IConfiguration>();

            // Setup JWT configuration - needs to match the actual controller usage
            _mockConfiguration.Setup(c => c["JwtSettings:Key"])
                .Returns("ThisIsASecretKeyForTestingPurposesOnly12345678");
            _mockConfiguration.Setup(c => c["JwtSettings:Issuer"])
                .Returns("TestIssuer");
            _mockConfiguration.Setup(c => c["JwtSettings:Audience"])
                .Returns("TestAudience");
            _mockConfiguration.Setup(c => c["JwtSettings:Duration"])
                .Returns("1");

            _controller = new AuthController(
                _mockLogger.Object,
                _mockMapper.Object,
                _mockUserManager.Object,
                _mockConfiguration.Object);
        }

        [Fact]
        public async Task Register_WithValidUserDto_ReturnsAccepted()
        {
            // Arrange
            var userDto = new UserDto
            {
                Email = "test@example.com",
                Password = "Test@123",
                FirstName = "Test",
                LastName = "User",
                Role = "User"
            };

            var apiUser = new ApiUser
            {
                Email = userDto.Email,
                UserName = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Role = userDto.Role
            };

            _mockMapper.Setup(m => m.Map<ApiUser>(userDto)).Returns(apiUser);
            _mockUserManager.Setup(u => u.CreateAsync(It.IsAny<ApiUser>(), userDto.Password))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(u => u.AddToRoleAsync(It.IsAny<ApiUser>(), userDto.Role))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(userDto);

            // Assert
            Assert.IsType<AcceptedResult>(result);
            _mockUserManager.Verify(u => u.CreateAsync(It.IsAny<ApiUser>(), userDto.Password), Times.Once);
            _mockUserManager.Verify(u => u.AddToRoleAsync(It.IsAny<ApiUser>(), userDto.Role), Times.Once);
        }

        [Fact]
        public async Task Register_WithInvalidRole_ReturnsBadRequest()
        {
            // Arrange
            var userDto = new UserDto
            {
                Email = "test@example.com",
                Password = "Test@123",
                FirstName = "Test",
                LastName = "User",
                Role = "InvalidRole"
            };

            // Act
            var result = await _controller.Register(userDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task Register_WithEmptyRole_ReturnsBadRequest()
        {
            // Arrange
            var userDto = new UserDto
            {
                Email = "test@example.com",
                Password = "Test@123",
                FirstName = "Test",
                LastName = "User",
                Role = ""
            };

            // Act
            var result = await _controller.Register(userDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_WhenUserCreationFails_ReturnsBadRequest()
        {
            // Arrange
            var userDto = new UserDto
            {
                Email = "test@example.com",
                Password = "Test@123",
                FirstName = "Test",
                LastName = "User",
                Role = "User"
            };

            var apiUser = new ApiUser { Email = userDto.Email };
            _mockMapper.Setup(m => m.Map<ApiUser>(userDto)).Returns(apiUser);

            var identityErrors = new List<IdentityError>
            {
                new IdentityError { Code = "DuplicateEmail", Description = "Email already exists" }
            };
            _mockUserManager.Setup(u => u.CreateAsync(It.IsAny<ApiUser>(), userDto.Password))
                .ReturnsAsync(IdentityResult.Failed(identityErrors.ToArray()));

            // Act
            var result = await _controller.Register(userDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsAuthResponse()
        {
            // Arrange
            var loginDto = new LoginUserDto
            {
                Email = "test@example.com",
                Password = "Test@123"
            };

            var apiUser = new ApiUser
            {
                Id = "user123",
                Email = loginDto.Email,
                UserName = loginDto.Email
            };

            _mockUserManager.Setup(u => u.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync(apiUser);
            _mockUserManager.Setup(u => u.CheckPasswordAsync(apiUser, loginDto.Password))
                .ReturnsAsync(true);
            _mockUserManager.Setup(u => u.GetRolesAsync(apiUser))
                .ReturnsAsync(new List<string> { "User" });
            _mockUserManager.Setup(u => u.GetClaimsAsync(apiUser))
                .ReturnsAsync(new List<System.Security.Claims.Claim>());

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);

            var authResponse = result.Value;
            Assert.Equal(loginDto.Email, authResponse.Email);
            Assert.Equal("user123", authResponse.UserId);
            Assert.NotNull(authResponse.Token);
            Assert.NotEmpty(authResponse.Token);
        }

        [Fact]
        public async Task Login_WithInvalidEmail_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginUserDto
            {
                Email = "nonexistent@example.com",
                Password = "Test@123"
            };

            _mockUserManager.Setup(u => u.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync((ApiUser)null);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<AuthResponse>>(result);
            Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginUserDto
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            var apiUser = new ApiUser
            {
                Email = loginDto.Email,
                UserName = loginDto.Email
            };

            _mockUserManager.Setup(u => u.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync(apiUser);
            _mockUserManager.Setup(u => u.CheckPasswordAsync(apiUser, loginDto.Password))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<AuthResponse>>(result);
            Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
        }
    }
}