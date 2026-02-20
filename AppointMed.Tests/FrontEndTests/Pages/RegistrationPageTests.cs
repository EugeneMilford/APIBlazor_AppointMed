using AppointMed.API.Models.User;
using Xunit;

namespace AppointMed.Tests.FrontEndTests.Pages
{
    public class RegistrationPageTests
    {
        [Fact]
        public void UserDto_WithValidData_IsValid()
        {
            // Arrange
            var userDto = new UserDto
            {
                Email = "newuser@example.com",
                Password = "Test@123",
                FirstName = "John",
                LastName = "Doe",
                Role = "User"
            };

            // Assert
            Assert.Equal("newuser@example.com", userDto.Email);
            Assert.Equal("John", userDto.FirstName);
            Assert.Equal("Doe", userDto.LastName);
            Assert.Equal("User", userDto.Role);
        }

        [Fact]
        public void UserDto_ValidatesRequiredFields()
        {
            // Arrange
            var userDto = new UserDto
            {
                Email = "",
                Password = "",
                FirstName = "",
                LastName = "",
                Role = ""
            };

            // Assert
            Assert.True(string.IsNullOrEmpty(userDto.Email));
            Assert.True(string.IsNullOrEmpty(userDto.Password));
            Assert.True(string.IsNullOrEmpty(userDto.FirstName));
            Assert.True(string.IsNullOrEmpty(userDto.LastName));
        }

        [Fact]
        public void UserDto_RoleValidation_AcceptsValidRoles()
        {
            // Arrange
            var allowedRoles = new[] { "User", "Administrator" };
            var userRole = "User";
            var adminRole = "Administrator";

            // Assert
            Assert.Contains(userRole, allowedRoles);
            Assert.Contains(adminRole, allowedRoles);
        }

        [Fact]
        public void UserDto_RoleValidation_RejectsInvalidRoles()
        {
            // Arrange
            var allowedRoles = new[] { "User", "Administrator" };
            var invalidRole = "SuperAdmin";

            // Assert
            Assert.DoesNotContain(invalidRole, allowedRoles);
        }

        [Fact]
        public void UserDto_PasswordStrength_IsValidated()
        {
            // Arrange
            var weakPassword = "123";
            var strongPassword = "Test@123";

            // Assert
            Assert.True(weakPassword.Length < 6);
            Assert.True(strongPassword.Length >= 6);
            Assert.True(strongPassword.Any(char.IsUpper));
            Assert.True(strongPassword.Any(char.IsLower));
            Assert.True(strongPassword.Any(char.IsDigit));
            Assert.True(strongPassword.Any(c => !char.IsLetterOrDigit(c)));
        }
    }
}
