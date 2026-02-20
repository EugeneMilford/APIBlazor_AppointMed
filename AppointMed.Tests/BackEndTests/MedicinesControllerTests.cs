using AppointMed.API.Controllers;
using AppointMed.API.Data;
using AppointMed.API.Models.Medicine;
using AppointMed.API.Repository.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AppointMed.Tests.BackEndTests
{
    public class MedicinesControllerTests
    {
        private readonly Mock<IMedicineRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<MedicinesController>> _mockLogger;
        private readonly MedicinesController _controller;

        public MedicinesControllerTests()
        {
            _mockRepository = new Mock<IMedicineRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<MedicinesController>>();

            _controller = new MedicinesController(
                _mockRepository.Object,
                _mockMapper.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task GetMedicines_ReturnsAllMedicines()
        {
            // Arrange
            var medicines = new List<MedicineDto>
            {
                new MedicineDto
                {
                    MedicineId = 1,
                    Name = "Aspirin",
                    Description = "Pain reliever",
                    Price = 5.99m,
                    Dosage = "500mg",
                    Manufacturer = "PharmaCorp",
                    IsAvailable = true
                },
                new MedicineDto
                {
                    MedicineId = 2,
                    Name = "Ibuprofen",
                    Description = "Anti-inflammatory",
                    Price = 7.99m,
                    Dosage = "200mg",
                    Manufacturer = "MediCo",
                    IsAvailable = false
                }
            };

            _mockRepository.Setup(r => r.GetAllMedicinesAsync()).ReturnsAsync(medicines);

            // Act
            var result = await _controller.GetMedicines();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<MedicineDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<MedicineDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetAvailableMedicines_ReturnsOnlyAvailableMedicines()
        {
            // Arrange
            var medicines = new List<MedicineDto>
            {
                new MedicineDto
                {
                    MedicineId = 1,
                    Name = "Aspirin",
                    IsAvailable = true,
                    Price = 5.99m,
                    Dosage = "500mg"
                },
                new MedicineDto
                {
                    MedicineId = 3,
                    Name = "Paracetamol",
                    IsAvailable = true,
                    Price = 4.99m,
                    Dosage = "650mg"
                }
            };

            _mockRepository.Setup(r => r.GetAvailableMedicinesAsync()).ReturnsAsync(medicines);

            // Act
            var result = await _controller.GetAvailableMedicines();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<MedicineDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<MedicineDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
            Assert.All(returnValue, m => Assert.True(m.IsAvailable));
        }

        [Fact]
        public async Task GetMedicine_WithValidId_ReturnsMedicine()
        {
            // Arrange
            var medicineDto = new MedicineDto
            {
                MedicineId = 1,
                Name = "Aspirin",
                Description = "Pain reliever",
                Price = 5.99m,
                Dosage = "500mg",
                Manufacturer = "PharmaCorp",
                IsAvailable = true
            };

            _mockRepository.Setup(r => r.GetMedicineDetailsAsync(1))
                .ReturnsAsync(medicineDto);

            // Act
            var result = await _controller.GetMedicine(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<MedicineDto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<MedicineDto>(okResult.Value);
            Assert.Equal(1, returnValue.MedicineId);
            Assert.Equal("Aspirin", returnValue.Name);
            Assert.Equal("500mg", returnValue.Dosage);
        }

        [Fact]
        public async Task GetMedicine_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetMedicineDetailsAsync(999))
                .ReturnsAsync((MedicineDto)null);

            // Act
            var result = await _controller.GetMedicine(999);

            // Assert
            var actionResult = Assert.IsType<ActionResult<MedicineDto>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task PostMedicine_WithValidData_ReturnsCreatedAtAction()
        {
            // Arrange
            var addMedicineDto = new AddMedicineDto
            {
                Name = "Aspirin",
                Description = "Pain reliever",
                Price = 5.99m,
                Dosage = "500mg",
                Manufacturer = "PharmaCorp",
                IsAvailable = true
            };

            var medicine = new Medicine
            {
                MedicineId = 0,
                Name = addMedicineDto.Name,
                Description = addMedicineDto.Description,
                Price = addMedicineDto.Price,
                Dosage = addMedicineDto.Dosage,
                Manufacturer = addMedicineDto.Manufacturer,
                IsAvailable = addMedicineDto.IsAvailable,
                CreatedAt = DateTime.UtcNow
            };

            var medicineDto = new MedicineDto
            {
                MedicineId = 1,
                Name = addMedicineDto.Name,
                Description = addMedicineDto.Description,
                Price = addMedicineDto.Price,
                Dosage = addMedicineDto.Dosage,
                Manufacturer = addMedicineDto.Manufacturer,
                IsAvailable = addMedicineDto.IsAvailable
            };

            _mockMapper.Setup(m => m.Map<Medicine>(addMedicineDto)).Returns(medicine);
            _mockMapper.Setup(m => m.Map<MedicineDto>(It.IsAny<Medicine>())).Returns(medicineDto);

            // Act
            var result = await _controller.PostMedicine(addMedicineDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<MedicineDto>>(result);
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal(nameof(_controller.GetMedicine), createdResult.ActionName);
        }

        [Fact]
        public async Task PutMedicine_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var updateMedicineDto = new UpdateMedicineDto
            {
                Id = 1,
                Name = "Aspirin Updated",
                Description = "Updated pain reliever",
                Price = 6.99m,
                Dosage = "500mg",
                Manufacturer = "PharmaCorp",
                IsAvailable = true
            };

            var existingMedicine = new Medicine
            {
                MedicineId = 1,
                Name = "Aspirin",
                Description = "Pain reliever",
                Price = 5.99m,
                Dosage = "500mg",
                Manufacturer = "PharmaCorp",
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            };

            _mockRepository.Setup(r => r.GetAsync(1)).ReturnsAsync(existingMedicine);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Medicine>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PutMedicine(1, updateMedicineDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutMedicine_WithMismatchedId_ReturnsBadRequest()
        {
            // Arrange
            var updateMedicineDto = new UpdateMedicineDto { Id = 1 };

            // Act
            var result = await _controller.PutMedicine(2, updateMedicineDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Medicine ID mismatch", badRequestResult.Value);
        }

        [Fact]
        public async Task PutMedicine_WithNonExistentMedicine_ReturnsNotFound()
        {
            // Arrange
            var updateMedicineDto = new UpdateMedicineDto { Id = 999 };

            _mockRepository.Setup(r => r.GetAsync(999)).ReturnsAsync((Medicine)null);

            // Act
            var result = await _controller.PutMedicine(999, updateMedicineDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteMedicine_WithValidId_ReturnsNoContent()
        {
            // Arrange
            _mockRepository.Setup(r => r.Exists(1)).ReturnsAsync(true);
            _mockRepository.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteMedicine(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteMedicine_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockRepository.Setup(r => r.Exists(999)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteMedicine(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}