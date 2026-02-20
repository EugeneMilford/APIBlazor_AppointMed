using AppointMed.API.Controllers;
using AppointMed.API.Data;
using AppointMed.API.Models.Prescription;
using AppointMed.API.Repository.Interface;
using AppointMed.API.Static;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace AppointMed.Tests.BackEndTests
{
    public class PrescriptionsControllerTests
    {
        private readonly Mock<IPrescriptionRepository> _mockRepository;
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<PrescriptionsController>> _mockLogger;
        private readonly PrescriptionsController _controller;

        public PrescriptionsControllerTests()
        {
            _mockRepository = new Mock<IPrescriptionRepository>();
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<PrescriptionsController>>();

            _controller = new PrescriptionsController(
                _mockRepository.Object,
                _mockAccountRepository.Object,
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
        public async Task GetPrescriptions_AsAdmin_ReturnsAllPrescriptions()
        {
            // Arrange
            var adminId = "admin123";
            SetupUserContext(adminId, "Administrator");

            var prescriptions = new List<PrescriptionDto>
            {
                new PrescriptionDto
                {
                    PrescriptionId = 1,
                    UserId = "user1",
                    MedicineName = "Aspirin",
                    Quantity = 30,
                    MedicinePrice = 10.00m,
                    TotalCost = 300.00m,
                    IsFulfilled = false
                },
                new PrescriptionDto
                {
                    PrescriptionId = 2,
                    UserId = "user2",
                    MedicineName = "Ibuprofen",
                    Quantity = 20,
                    MedicinePrice = 15.00m,
                    TotalCost = 300.00m,
                    IsFulfilled = true
                }
            };

            _mockRepository.Setup(r => r.GetAllPrescriptionsAsync())
                .ReturnsAsync(prescriptions);

            // Act
            var result = await _controller.GetPrescriptions();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<PrescriptionDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<PrescriptionDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetMyPrescriptions_ReturnsUserPrescriptions()
        {
            // Arrange
            var userId = "user123";
            SetupUserContext(userId);

            var prescriptions = new List<PrescriptionDto>
            {
                new PrescriptionDto
                {
                    PrescriptionId = 1,
                    UserId = userId,
                    MedicineName = "Aspirin",
                    Quantity = 30,
                    MedicinePrice = 10.00m,
                    TotalCost = 300.00m,
                    Instructions = "Take twice daily"
                },
                new PrescriptionDto
                {
                    PrescriptionId = 2,
                    UserId = userId,
                    MedicineName = "Vitamin D",
                    Quantity = 60,
                    MedicinePrice = 8.00m,
                    TotalCost = 480.00m,
                    Instructions = "Take once daily with food"
                }
            };

            _mockRepository.Setup(r => r.GetPrescriptionsByUserIdAsync(userId))
                .ReturnsAsync(prescriptions);

            // Act
            var result = await _controller.GetMyPrescriptions();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<PrescriptionDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<PrescriptionDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
            Assert.All(returnValue, p => Assert.Equal(userId, p.UserId));
        }

        [Fact]
        public async Task GetMyPrescriptions_WithoutUserId_ReturnsUnauthorized()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            // Act
            var result = await _controller.GetMyPrescriptions();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<PrescriptionDto>>>(result);
            Assert.IsType<UnauthorizedResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetPrescription_WithValidIdAndOwner_ReturnsPrescription()
        {
            // Arrange
            var userId = "user123";
            SetupUserContext(userId);

            var prescriptionDto = new PrescriptionDto
            {
                PrescriptionId = 1,
                UserId = userId,
                AppointmentId = 5,
                MedicineId = 10,
                MedicineName = "Aspirin",
                MedicinePrice = 10.00m,
                Quantity = 30,
                TotalCost = 300.00m,
                Instructions = "Take twice daily after meals",
                PrescribedDate = DateTime.UtcNow,
                IsFulfilled = false
            };

            _mockRepository.Setup(r => r.GetPrescriptionAsync(1))
                .ReturnsAsync(prescriptionDto);

            // Act
            var result = await _controller.GetPrescription(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<PrescriptionDto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<PrescriptionDto>(okResult.Value);
            Assert.Equal(1, returnValue.PrescriptionId);
            Assert.Equal("Aspirin", returnValue.MedicineName);
            Assert.Equal(30, returnValue.Quantity);
        }

        [Fact]
        public async Task GetPrescription_WithValidIdButNotOwner_ReturnsForbid()
        {
            // Arrange
            var userId = "user123";
            SetupUserContext(userId);

            var prescriptionDto = new PrescriptionDto
            {
                PrescriptionId = 1,
                UserId = "differentUser",
                MedicineName = "Aspirin",
                Quantity = 30
            };

            _mockRepository.Setup(r => r.GetPrescriptionAsync(1))
                .ReturnsAsync(prescriptionDto);

            // Act
            var result = await _controller.GetPrescription(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<PrescriptionDto>>(result);
            Assert.IsType<ForbidResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetPrescription_AsAdmin_ReturnsAnyPrescription()
        {
            // Arrange
            var adminId = "admin123";
            SetupUserContext(adminId, "Administrator");

            var prescriptionDto = new PrescriptionDto
            {
                PrescriptionId = 1,
                UserId = "differentUser",
                UserName = "John Doe",
                MedicineName = "Aspirin",
                Quantity = 30,
                TotalCost = 300.00m
            };

            _mockRepository.Setup(r => r.GetPrescriptionAsync(1))
                .ReturnsAsync(prescriptionDto);

            // Act
            var result = await _controller.GetPrescription(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<PrescriptionDto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<PrescriptionDto>(okResult.Value);
            Assert.Equal(1, returnValue.PrescriptionId);
            Assert.Equal("John Doe", returnValue.UserName);
        }

        [Fact]
        public async Task GetPrescription_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var userId = "user123";
            SetupUserContext(userId);

            _mockRepository.Setup(r => r.GetPrescriptionAsync(999))
                .ReturnsAsync((PrescriptionDto)null);

            // Act
            var result = await _controller.GetPrescription(999);

            // Assert
            var actionResult = Assert.IsType<ActionResult<PrescriptionDto>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetPrescriptionsByAppointment_AsRegularUser_ReturnsFilteredPrescriptions()
        {
            // Arrange
            var userId = "user123";
            SetupUserContext(userId);

            var allPrescriptions = new List<PrescriptionDto>
            {
                new PrescriptionDto
                {
                    PrescriptionId = 1,
                    UserId = userId,
                    AppointmentId = 5,
                    MedicineName = "Aspirin",
                    Quantity = 30
                },
                new PrescriptionDto
                {
                    PrescriptionId = 2,
                    UserId = "differentUser",
                    AppointmentId = 5,
                    MedicineName = "Ibuprofen",
                    Quantity = 20
                }
            };

            _mockRepository.Setup(r => r.GetPrescriptionsByAppointmentIdAsync(5))
                .ReturnsAsync(allPrescriptions);

            // Act
            var result = await _controller.GetPrescriptionsByAppointment(5);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<PrescriptionDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<PrescriptionDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(userId, returnValue[0].UserId);
        }

        [Fact]
        public async Task GetPrescriptionsByAppointment_AsAdmin_ReturnsAllAppointmentPrescriptions()
        {
            // Arrange
            var adminId = "admin123";
            SetupUserContext(adminId, "Administrator");

            var prescriptions = new List<PrescriptionDto>
            {
                new PrescriptionDto
                {
                    PrescriptionId = 1,
                    UserId = "user1",
                    AppointmentId = 5,
                    MedicineName = "Aspirin",
                    Quantity = 30,
                    TotalCost = 300.00m,
                    IsFulfilled = true,
                    FulfilledDate = DateTime.UtcNow.AddDays(-1)
                },
                new PrescriptionDto
                {
                    PrescriptionId = 2,
                    UserId = "user2",
                    AppointmentId = 5,
                    MedicineName = "Paracetamol",
                    Quantity = 20,
                    TotalCost = 200.00m,
                    IsFulfilled = false
                }
            };

            _mockRepository.Setup(r => r.GetPrescriptionsByAppointmentIdAsync(5))
                .ReturnsAsync(prescriptions);

            // Act
            var result = await _controller.GetPrescriptionsByAppointment(5);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<PrescriptionDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<PrescriptionDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetPrescriptionsByAppointment_VerifiesPrescriptionProperties()
        {
            // Arrange
            var userId = "user123";
            SetupUserContext(userId);

            var prescriptions = new List<PrescriptionDto>
            {
                new PrescriptionDto
                {
                    PrescriptionId = 1,
                    AppointmentId = 5,
                    MedicineId = 100,
                    MedicineName = "Aspirin 500mg",
                    MedicinePrice = 10.00m,
                    UserId = userId,
                    UserName = "John Smith",
                    Quantity = 30,
                    TotalCost = 300.00m,
                    Instructions = "Take 1 tablet twice daily with food",
                    PrescribedDate = new DateTime(2026, 2, 15),
                    IsFulfilled = true,
                    FulfilledDate = new DateTime(2026, 2, 16)
                }
            };

            _mockRepository.Setup(r => r.GetPrescriptionsByAppointmentIdAsync(5))
                .ReturnsAsync(prescriptions);

            // Act
            var result = await _controller.GetPrescriptionsByAppointment(5);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<PrescriptionDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<PrescriptionDto>>(okResult.Value);

            var prescription = returnValue.First();
            Assert.Equal(1, prescription.PrescriptionId);
            Assert.Equal(5, prescription.AppointmentId);
            Assert.Equal(100, prescription.MedicineId);
            Assert.Equal("Aspirin 500mg", prescription.MedicineName);
            Assert.Equal(10.00m, prescription.MedicinePrice);
            Assert.Equal(30, prescription.Quantity);
            Assert.Equal(300.00m, prescription.TotalCost);
            Assert.Equal("Take 1 tablet twice daily with food", prescription.Instructions);
            Assert.True(prescription.IsFulfilled);
            Assert.NotNull(prescription.FulfilledDate);
        }
    }
}
