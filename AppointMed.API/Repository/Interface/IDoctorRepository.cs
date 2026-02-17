using AppointMed.API.Data;
using AppointMed.API.Models.Doctor;

namespace AppointMed.API.Repository.Interface
{
    public interface IDoctorRepository : IGenericRepository<Doctor>
    {
        Task<List<DoctorDto>> GetAllDoctorsAsync();
        Task<DoctorDto> GetDoctorDetailsAsync(int id);
    }
}
