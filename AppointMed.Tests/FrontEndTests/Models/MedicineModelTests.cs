using AppointMed.API.Models.Medicine;
using Xunit;

namespace AppointMed.Tests.FrontEndTests.Models
{
    public class MedicineModelTests
    {
        [Fact]
        public void MedicineDto_DisplaysBasicInformation()
        {
            // Arrange
            var medicine = new MedicineDto
            {
                MedicineId = 1,
                Name = "Aspirin",
                Description = "Pain reliever",
                Price = 5.99m,
                Dosage = "500mg",
                Manufacturer = "PharmaCorp",
                IsAvailable = true
            };

            // Assert
            Assert.Equal("Aspirin", medicine.Name);
            Assert.Equal(5.99m, medicine.Price);
            Assert.Equal("500mg", medicine.Dosage);
            Assert.True(medicine.IsAvailable);
        }

        [Fact]
        public void AddMedicineDto_RequiresBasicFields()
        {
            // Arrange
            var addMedicine = new AddMedicineDto
            {
                Name = "Ibuprofen",
                Price = 7.99m,
                Dosage = "200mg",
                IsAvailable = true
            };

            // Assert
            Assert.NotNull(addMedicine.Name);
            Assert.True(addMedicine.Price > 0);
            Assert.True(addMedicine.IsAvailable);
        }

        [Fact]
        public void MedicineDto_PriceFormat_IsCorrect()
        {
            // Arrange
            var medicine = new MedicineDto
            {
                MedicineId = 1,
                Name = "Paracetamol",
                Price = 4.99m
            };

            // Act
            var formattedPrice = medicine.Price.ToString("C");

            // Assert
            Assert.Contains("4", formattedPrice);
            Assert.Contains("99", formattedPrice);
        }
    }
}
