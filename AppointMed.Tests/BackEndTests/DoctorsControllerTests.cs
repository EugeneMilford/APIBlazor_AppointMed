using AppointMed.API.Controllers;
using AppointMed.API.Data;
using AppointMed.API.Models.Doctor;
using AppointMed.API.Repository.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AppointMed.Tests.BackEndTests
{
    public class DoctorsControllerTests
    {
        private readonly Mock<IDoctorRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<DoctorsController>> _mockLogger;
        private readonly DoctorsController _controller;

        public DoctorsControllerTests()
        {
            _mockRepository = new Mock<IDoctorRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<DoctorsController>>();

            _controller = new DoctorsController(
                _mockRepository.Object,
                _mockMapper.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task GetDoctors_ReturnsListOfDoctors()
        {
            // Arrange
            var doctors = new List<DoctorDto>
            {
                new DoctorDto
                {
                    DoctorId = 1,
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "john.smith@hospital.com",
                    Specialization = "Cardiology",
                    IsActive = true
                },
                new DoctorDto
                {
                    DoctorId = 2,
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "jane.doe@hospital.com",
                    Specialization = "Neurology",
                    IsActive = true
                }
            };

            _mockRepository.Setup(r => r.GetAllDoctorsAsync()).ReturnsAsync(doctors);

            // Act
            var result = await _controller.GetDoctors();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<DoctorDto>>>(result);
            var returnValue = Assert.IsType<List<DoctorDto>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetDoctor_WithValidId_ReturnsDoctor()
        {
            // Arrange
            var doctorDto = new DoctorDto
            {
                DoctorId = 1,
                FirstName = "John",
                LastName = "Smith",
                Email = "john.smith@hospital.com",
                Specialization = "Cardiology",
                IsActive = true
            };

            _mockRepository.Setup(r => r.GetDoctorDetailsAsync(1))
                .ReturnsAsync(doctorDto);

            // Act
            var result = await _controller.GetDoctor(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<DoctorDto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<DoctorDto>(okResult.Value);
            Assert.Equal(1, returnValue.DoctorId);
        }

        [Fact]
        public async Task GetDoctor_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetDoctorDetailsAsync(999))
                .ReturnsAsync((DoctorDto)null);

            // Act
            var result = await _controller.GetDoctor(999);

            // Assert
            var actionResult = Assert.IsType<ActionResult<DoctorDto>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task PostDoctor_WithValidData_ReturnsCreatedAtAction()
        {
            // Arrange
            var addDoctorDto = new AddDoctorDto
            {
                FirstName = "John",
                LastName = "Smith",
                Specialization = "Cardiology",
                PhoneNumber = "1234567890",
                Email = "dr.smith@example.com"
            };

            var doctor = new Doctor
            {
                DoctorId = 0, // Will be set by database
                FirstName = "John",
                LastName = "Smith",
                Specialization = "Cardiology",
                PhoneNumber = "1234567890",
                Email = "dr.smith@example.com"
            };

            var doctorDto = new DoctorDto
            {
                DoctorId = 1,
                FirstName = "John",
                LastName = "Smith",
                Email = "dr.smith@example.com",
                Specialization = "Cardiology",
                IsActive = true
            };

            _mockMapper.Setup(m => m.Map<Doctor>(addDoctorDto)).Returns(doctor);
            _mockMapper.Setup(m => m.Map<DoctorDto>(It.IsAny<Doctor>())).Returns(doctorDto);

            // Act
            var result = await _controller.PostDoctor(addDoctorDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<DoctorDto>>(result);
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal(nameof(_controller.GetDoctor), createdResult.ActionName);
        }

        [Fact]
        public async Task PutDoctor_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var updateDoctorDto = new UpdateDoctorDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Smith",
                Email = "john.smith@hospital.com",
                Specialization = "Cardiology Updated"
            };

            var existingDoctor = new Doctor
            {
                DoctorId = 1,
                FirstName = "John",
                LastName = "Smith",
                Email = "john.smith@hospital.com",
                Specialization = "Cardiology"
            };

            _mockRepository.Setup(r => r.GetAsync(1)).ReturnsAsync(existingDoctor);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Doctor>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.Exists(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.PutDoctor(1, updateDoctorDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutDoctor_WithMismatchedId_ReturnsBadRequest()
        {
            // Arrange
            var updateDoctorDto = new UpdateDoctorDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Smith",
                Email = "john@example.com"
            };

            // Act
            var result = await _controller.PutDoctor(2, updateDoctorDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Doctor ID mismatch", badRequestResult.Value);
        }

        [Fact]
        public async Task PutDoctor_WithNonExistentDoctor_ReturnsNotFound()
        {
            // Arrange
            var updateDoctorDto = new UpdateDoctorDto
            {
                Id = 999,
                FirstName = "John",
                LastName = "Smith",
                Email = "john@example.com"
            };

            _mockRepository.Setup(r => r.GetAsync(999)).ReturnsAsync((Doctor)null);

            // Act
            var result = await _controller.PutDoctor(999, updateDoctorDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteDoctor_WithValidId_ReturnsNoContent()
        {
            // Arrange
            // IMPORTANT: The controller uses Exists() to check if doctor exists, not GetAsync()
            _mockRepository.Setup(r => r.Exists(1)).ReturnsAsync(true);
            _mockRepository.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteDoctor(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteDoctor_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            // The controller checks Exists() first
            _mockRepository.Setup(r => r.Exists(999)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteDoctor(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}