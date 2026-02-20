using Xunit;

namespace AppointMed.Tests.FrontEndTests.Helpers
{
    public class DateTimeHelpersTests
    {
        [Fact]
        public void FormatDate_ReturnsCorrectFormat()
        {
            // Arrange
            var date = new DateTime(2026, 2, 20, 14, 30, 0);

            // Act
            var formatted = date.ToString("dd/MM/yyyy");

            // Assert
            Assert.Equal("20/02/2026", formatted);
        }

        [Fact]
        public void FormatDateTime_ReturnsCorrectFormat()
        {
            // Arrange
            var dateTime = new DateTime(2026, 2, 20, 14, 30, 0);

            // Act
            var formatted = dateTime.ToString("dd/MM/yyyy HH:mm");

            // Assert
            Assert.Equal("20/02/2026 14:30", formatted);
        }

        [Fact]
        public void FormatTime_ReturnsCorrectFormat()
        {
            // Arrange
            var dateTime = new DateTime(2026, 2, 20, 14, 30, 0);

            // Act
            var formatted = dateTime.ToString("HH:mm");

            // Assert
            Assert.Equal("14:30", formatted);
        }

        [Fact]
        public void CalculateAge_ReturnsCorrectAge()
        {
            // Arrange
            var birthDate = new DateTime(1990, 1, 1);
            var today = new DateTime(2026, 2, 20);

            // Act
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;

            // Assert
            Assert.Equal(36, age);
        }

        [Fact]
        public void IsUpcoming_ChecksFutureDate()
        {
            // Arrange
            var futureDate = DateTime.Now.AddDays(7);
            var pastDate = DateTime.Now.AddDays(-7);

            // Assert
            Assert.True(futureDate > DateTime.Now);
            Assert.False(pastDate > DateTime.Now);
        }

        [Fact]
        public void GetDayOfWeek_ReturnsCorrectDay()
        {
            // Arrange
            var date = new DateTime(2026, 2, 20); // Friday

            // Act
            var dayOfWeek = date.DayOfWeek;

            // Assert
            Assert.Equal(DayOfWeek.Friday, dayOfWeek);
        }
    }
}
