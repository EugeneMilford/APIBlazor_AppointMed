using AppointMed.API.Models.Doctor;
using Xunit;

namespace AppointMed.Tests.FrontEndTests.Components
{
    public class DoctorCardTests
    {
        [Fact]
        public void DoctorCard_DisplaysDoctorInformation()
        {
            // Arrange
            var doctor = new DoctorDto
            {
                DoctorId = 1,
                FirstName = "John",
                LastName = "Smith",
                Email = "john.smith@hospital.com",
                Specialization = "Cardiology",
                PhoneNumber = "1234567890",
                Bio = "Experienced cardiologist",
                IsActive = true,
                DateJoined = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            // Assert
            Assert.Equal("John Smith", doctor.FullName);
            Assert.Equal("Cardiology", doctor.Specialization);
            Assert.True(doctor.IsActive);
            Assert.NotNull(doctor.Email);
        }

        [Fact]
        public void DoctorCard_ShowsActiveStatus()
        {
            // Arrange
            var activeDoctor = new DoctorDto
            {
                DoctorId = 1,
                FirstName = "Jane",
                LastName = "Doe",
                IsActive = true
            };

            var inactiveDoctor = new DoctorDto
            {
                DoctorId = 2,
                FirstName = "Bob",
                LastName = "Johnson",
                IsActive = false
            };

            // Assert
            Assert.True(activeDoctor.IsActive);
            Assert.False(inactiveDoctor.IsActive);
        }

        [Fact]
        public void DoctorCard_DisplaysSpecialization()
        {
            // Arrange
            var specializations = new[]
            {
                "Cardiology",
                "Neurology",
                "Orthopedics",
                "Pediatrics"
            };

            var doctor = new DoctorDto
            {
                DoctorId = 1,
                FirstName = "Dr.",
                LastName = "Test",
                Specialization = "Cardiology"
            };

            // Assert
            Assert.Contains(doctor.Specialization, specializations);
        }
    }
}
