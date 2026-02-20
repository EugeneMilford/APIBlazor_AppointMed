using AppointMed.API.Models.Appointment;
using Xunit;

namespace AppointMed.Tests.FrontEndTests.Models
{
    public class AppointmentModelTests
    {
        [Fact]
        public void AddAppointmentDto_RequiresPatientInformation()
        {
            // Arrange
            var appointment = new AddAppointmentDto
            {
                PatientFirstName = "John",
                PatientLastName = "Doe",
                PatientEmail = "john.doe@example.com",
                PatientPhoneNumber = "1234567890",
                AppointmentDateTime = DateTime.Now.AddDays(1),
                DoctorId = 1,
                StatusId = 1
            };

            // Assert
            Assert.NotNull(appointment.PatientFirstName);
            Assert.NotNull(appointment.PatientLastName);
            Assert.NotNull(appointment.PatientEmail);
            Assert.True(appointment.AppointmentDateTime > DateTime.Now);
        }

        [Fact]
        public void AppointmentDto_DisplaysFormattedDate()
        {
            // Arrange
            var appointmentDate = new DateTime(2026, 3, 15, 14, 30, 0);
            var appointment = new AppointmentDto
            {
                AppointmentId = 1,
                AppointmentDateTime = appointmentDate,
                DoctorName = "Dr. Smith"
            };

            // Assert
            Assert.Equal(2026, appointment.AppointmentDateTime.Year);
            Assert.Equal(3, appointment.AppointmentDateTime.Month);
            Assert.Equal(15, appointment.AppointmentDateTime.Day);
            Assert.Equal(14, appointment.AppointmentDateTime.Hour);
            Assert.Equal(30, appointment.AppointmentDateTime.Minute);
        }

        [Fact]
        public void UpdateAppointmentDto_AllowsStatusChange()
        {
            // Arrange
            var updateAppointment = new UpdateAppointmentDto
            {
                Id = 1,
                StatusId = 2,
                Notes = "Patient confirmed"
            };

            // Assert
            Assert.Equal(2, updateAppointment.StatusId);
            Assert.Equal("Patient confirmed", updateAppointment.Notes);
        }

        [Fact]
        public void AppointmentDto_FullPatientName_CombinesNames()
        {
            // Arrange
            var appointment = new AppointmentDto
            {
                AppointmentId = 1,
                PatientFirstName = "John",
                PatientLastName = "Doe"
            };

            // Act
            var fullName = $"{appointment.PatientFirstName} {appointment.PatientLastName}";

            // Assert
            Assert.Equal("John Doe", fullName);
        }
    }
}
