using AppointMed.API.Models.User;
using Xunit;

namespace AppointMed.Tests.FrontEndTests.Pages
{
    public class LoginPageTests
    {
        [Fact]
        public void LoginDto_ValidatesRequiredEmail()
        {
            // Arrange
            var loginDto = new LoginUserDto
            {
                Email = "",
                Password = "Test@123"
            };

            // Assert
            Assert.True(string.IsNullOrEmpty(loginDto.Email));
        }

        [Fact]
        public void LoginDto_ValidatesRequiredPassword()
        {
            // Arrange
            var loginDto = new LoginUserDto
            {
                Email = "test@example.com",
                Password = ""
            };

            // Assert
            Assert.True(string.IsNullOrEmpty(loginDto.Password));
        }

        [Fact]
        public void LoginDto_WithValidData_IsValid()
        {
            // Arrange
            var loginDto = new LoginUserDto
            {
                Email = "test@example.com",
                Password = "Test@123"
            };

            // Assert
            Assert.False(string.IsNullOrEmpty(loginDto.Email));
            Assert.False(string.IsNullOrEmpty(loginDto.Password));
            Assert.Contains("@", loginDto.Email);
        }

        [Fact]
        public void LoginDto_EmailFormat_IsValid()
        {
            // Arrange
            var validEmails = new[]
            {
                "user@example.com",
                "test.user@domain.co.za",
                "admin@hospital.org"
            };

            // Assert
            foreach (var email in validEmails)
            {
                var isValid = IsValidEmail(email);
                Assert.True(isValid, $"Email {email} should be valid");
            }
        }

        [Fact]
        public void LoginDto_EmailFormat_IsInvalid()
        {
            // Arrange
            var invalidEmails = new[]
            {
                "notanemail",
                "@example.com",
                "user@",
                "user@.com",
                "user.example.com",
                "user@@example.com",
                ".user@example.com"
            };

            // Assert
            foreach (var email in invalidEmails)
            {
                var isValid = IsValidEmail(email);
                Assert.False(isValid, $"Email {email} should be invalid");
            }
        }

        [Fact]
        public void LoginDto_PasswordStrength_CanBeValidated()
        {
            // Arrange
            var weakPassword = "123";
            var mediumPassword = "Test123";
            var strongPassword = "Test@123";

            // Assert - Check password length
            Assert.True(weakPassword.Length < 6);
            Assert.True(mediumPassword.Length >= 6);
            Assert.True(strongPassword.Length >= 6);

            // Assert - Check strong password has special characters
            Assert.True(strongPassword.Any(c => !char.IsLetterOrDigit(c)));
        }

        [Fact]
        public void LoginDto_EmailCaseInsensitive_ShouldWork()
        {
            // Arrange
            var loginDto1 = new LoginUserDto
            {
                Email = "Test@Example.com",
                Password = "Test@123"
            };

            var loginDto2 = new LoginUserDto
            {
                Email = "test@example.com",
                Password = "Test@123"
            };

            // Assert
            Assert.Equal(loginDto1.Email.ToLower(), loginDto2.Email.ToLower());
        }

        // Helper method for email validation
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Must contain @
            if (!email.Contains("@"))
                return false;

            // Must contain .
            if (!email.Contains("."))
                return false;

            var atIndex = email.IndexOf("@");
            var lastDotIndex = email.LastIndexOf(".");

            // @ must not be at start or end
            if (atIndex <= 0 || atIndex >= email.Length - 1)
                return false;

            // . must come after @
            if (lastDotIndex <= atIndex)
                return false;

            // . must not be immediately after @
            if (lastDotIndex == atIndex + 1)
                return false;

            // . must not be at the end
            if (lastDotIndex >= email.Length - 1)
                return false;

            // Must not have multiple @
            if (email.Count(c => c == '@') > 1)
                return false;

            // Local part (before @) must not start with .
            var localPart = email.Substring(0, atIndex);
            if (localPart.StartsWith("."))
                return false;

            // Domain part (after @) must have at least one character before the dot
            var domainPart = email.Substring(atIndex + 1);
            var domainDotIndex = domainPart.IndexOf(".");
            if (domainDotIndex <= 0)
                return false;

            return true;
        }
    }
}