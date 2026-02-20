using AppointMed.API.Models.Doctor;
using Xunit;

namespace AppointMed.Tests.FrontEndTests.Models
{
    public class DoctorModelTests
    {
        [Fact]
        public void DoctorDto_FullName_CombinesFirstAndLastName()
        {
            // Arrange
            var doctor = new DoctorDto
            {
                FirstName = "Jane",
                LastName = "Smith"
            };

            // Assert
            Assert.Equal("Jane Smith", doctor.FullName);
        }

        [Fact]
        public void AddDoctorDto_RequiresEmail()
        {
            // Arrange
            var addDoctor = new AddDoctorDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@hospital.com",
                Specialization = "Cardiology"
            };

            // Assert
            Assert.NotNull(addDoctor.Email);
            Assert.Contains("@", addDoctor.Email);
        }

        [Fact]
        public void UpdateDoctorDto_AllowsSpecializationUpdate()
        {
            // Arrange
            var updateDoctor = new UpdateDoctorDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@hospital.com",
                Specialization = "Cardiology - Interventional"
            };

            // Assert
            Assert.Equal("Cardiology - Interventional", updateDoctor.Specialization);
        }

        [Fact]
        public void DoctorDto_CalculatesYearsOfExperience()
        {
            // Arrange
            var doctor = new DoctorDto
            {
                DoctorId = 1,
                FirstName = "John",
                LastName = "Smith",
                DateJoined = DateTime.UtcNow.AddYears(-5)
            };

            // Act
            var yearsOfExperience = (DateTime.UtcNow - doctor.DateJoined).Days / 365;

            // Assert
            Assert.True(yearsOfExperience >= 4 && yearsOfExperience <= 5);
        }
    }
}
