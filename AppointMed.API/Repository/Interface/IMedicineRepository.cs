using AppointMed.API.Data;
using AppointMed.API.Models.Medicine;

namespace AppointMed.API.Repository.Interface
{
    public interface IMedicineRepository : IGenericRepository<Medicine>
    {
        Task<List<MedicineDto>> GetAllMedicinesAsync();
        Task<MedicineDto> GetMedicineDetailsAsync(int id);
        Task<List<MedicineDto>> GetAvailableMedicinesAsync();
    }
}
