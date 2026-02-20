using AppointMed.API.Controllers;
using AppointMed.API.Data;
using AppointMed.API.Models.Appointment;
using AppointMed.API.Repository.Interface;
using AppointMed.API.Static;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;
using Assert = Xunit.Assert;

namespace AppointMed.Tests.BackEndTests
{
    public class AppointmentsControllerTests
    {
        private readonly Mock<IAppointmentRepository> _mockRepository;
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly Mock<IPrescriptionRepository> _mockPrescriptionRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<AppointmentsController>> _mockLogger;
        private readonly AppointmentsController _controller;

        public AppointmentsControllerTests()
        {
            _mockRepository = new Mock<IAppointmentRepository>();
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockPrescriptionRepository = new Mock<IPrescriptionRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<AppointmentsController>>();

            _controller = new AppointmentsController(
                _mockRepository.Object,
                _mockAccountRepository.Object,
                _mockPrescriptionRepository.Object,
                _mockMapper.Object,
                _mockLogger.Object);
        }

        private void SetupUserContext(string userId, string role = "User")
        {
            var claims = new List<Claim>
            {
                new Claim(CustomClaimTypes.Uid, userId),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public async Task GetAppointments_AsRegularUser_ReturnsUserAppointments()
        {
            // Arrange
            var userId = "user123";
            SetupUserContext(userId);

            var appointments = new List<AppointmentDto>
            {
                new AppointmentDto
                {
                    AppointmentId = 1,
                    UserId = userId,
                    PatientFirstName = "John",
                    PatientLastName = "Doe",
                    AppointmentDateTime = DateTime.UtcNow.AddDays(1),
                    DoctorId = 1,
                    DoctorName = "Dr. Smith",
                    StatusId = 1,
                    StatusName = "Confirmed"
                },
                new AppointmentDto
                {
                    AppointmentId = 2,
                    UserId = userId,
                    PatientFirstName = "John",
                    PatientLastName = "Doe",
                    AppointmentDateTime = DateTime.UtcNow.AddDays(7),
                    DoctorId = 2,
                    DoctorName = "Dr. Jones",
                    StatusId = 1,
                    StatusName = "Confirmed"
                }
            };

            _mockRepository.Setup(r => r.GetAppointmentsByUserIdAsync(userId))
                .ReturnsAsync(appointments);

            // Act
            var result = await _controller.GetAppointments();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<AppointmentDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<AppointmentDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetAppointments_AsAdmin_ReturnsAllAppointments()
        {
            // Arrange
            var userId = "admin123";
            SetupUserContext(userId, "Administrator");

            var appointments = new List<AppointmentDto>
            {
                new AppointmentDto
                {
                    AppointmentId = 1,
                    UserId = "user1",
                    PatientFirstName = "Jane",
                    PatientLastName = "Smith",
                    DoctorName = "Dr. Brown"
                },
                new AppointmentDto
                {
                    AppointmentId = 2,
                    UserId = "user2",
                    PatientFirstName = "Bob",
                    PatientLastName = "Johnson",
                    DoctorName = "Dr. White"
                }
            };

            _mockRepository.Setup(r => r.GetAllAppointmentsAsync())
                .ReturnsAsync(appointments);

            // Act
            var result = await _controller.GetAppointments();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<AppointmentDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<AppointmentDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetAppointment_WithValidIdAndOwner_ReturnsAppointment()
        {
            // Arrange
            var userId = "user123";
            SetupUserContext(userId);

            var appointmentDto = new AppointmentDto
            {
                AppointmentId = 1,
                UserId = userId,
                PatientFirstName = "John",
                PatientLastName = "Doe",
                PatientEmail = "john.doe@example.com",
                PatientPhoneNumber = "1234567890",
                AppointmentDateTime = DateTime.UtcNow.AddDays(1),
                AppointmentType = "Consultation",
                Notes = "Regular checkup",
                DoctorId = 1,
                DoctorName = "Dr. Smith",
                StatusId = 1,
                StatusName = "Confirmed"
            };

            _mockRepository.Setup(r => r.GetAppointmentAsync(1))
                .ReturnsAsync(appointmentDto);

            // Act
            var result = await _controller.GetAppointment(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<AppointmentDto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<AppointmentDto>(okResult.Value);
            Assert.Equal(1, returnValue.AppointmentId);
            Assert.Equal("John", returnValue.PatientFirstName);
            Assert.Equal("Dr. Smith", returnValue.DoctorName);
        }

        [Fact]
        public async Task GetAppointment_WithValidIdButNotOwner_ReturnsForbid()
        {
            // Arrange
            var userId = "user123";
            SetupUserContext(userId);

            var appointmentDto = new AppointmentDto
            {
                AppointmentId = 1,
                UserId = "differentUser",
                PatientFirstName = "Jane",
                PatientLastName = "Smith",
                DoctorName = "Dr. Smith"
            };

            _mockRepository.Setup(r => r.GetAppointmentAsync(1))
                .ReturnsAsync(appointmentDto);

            // Act
            var result = await _controller.GetAppointment(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<AppointmentDto>>(result);
            Assert.IsType<ForbidResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetAppointment_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var userId = "user123";
            SetupUserContext(userId);

            _mockRepository.Setup(r => r.GetAppointmentAsync(999))
                .ReturnsAsync((AppointmentDto)null);

            // Act
            var result = await _controller.GetAppointment(999);

            // Assert
            var actionResult = Assert.IsType<ActionResult<AppointmentDto>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetAppointmentsByDoctor_AsRegularUser_ReturnsFilteredAppointments()
        {
            // Arrange
            var userId = "user123";
            SetupUserContext(userId);

            var allAppointments = new List<AppointmentDto>
            {
                new AppointmentDto
                {
                    AppointmentId = 1,
                    UserId = userId,
                    DoctorId = 5,
                    DoctorName = "Dr. Anderson",
                    PatientFirstName = "John",
                    PatientLastName = "Doe"
                },
                new AppointmentDto
                {
                    AppointmentId = 2,
                    UserId = "differentUser",
                    DoctorId = 5,
                    DoctorName = "Dr. Anderson",
                    PatientFirstName = "Jane",
                    PatientLastName = "Smith"
                }
            };

            _mockRepository.Setup(r => r.GetAppointmentsByDoctorAsync(5))
                .ReturnsAsync(allAppointments);

            // Act
            var result = await _controller.GetAppointmentsByDoctor(5);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<AppointmentDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<AppointmentDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(userId, returnValue[0].UserId);
        }

        [Fact]
        public async Task GetAppointmentsByDoctor_AsAdmin_ReturnsAllDoctorAppointments()
        {
            // Arrange
            var adminId = "admin123";
            SetupUserContext(adminId, "Administrator");

            var appointments = new List<AppointmentDto>
            {
                new AppointmentDto
                {
                    AppointmentId = 1,
                    UserId = "user1",
                    DoctorId = 5,
                    DoctorName = "Dr. Anderson"
                },
                new AppointmentDto
                {
                    AppointmentId = 2,
                    UserId = "user2",
                    DoctorId = 5,
                    DoctorName = "Dr. Anderson"
                }
            };

            _mockRepository.Setup(r => r.GetAppointmentsByDoctorAsync(5))
                .ReturnsAsync(appointments);

            // Act
            var result = await _controller.GetAppointmentsByDoctor(5);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<AppointmentDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<AppointmentDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }
    }
}
