using AppointMed.API.Data;
using AppointMed.API.Models.Appointment;

namespace AppointMed.API.Repository.Interface
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<List<AppointmentDto>> GetAllAppointmentsAsync();
        Task<AppointmentDto> GetAppointmentAsync(int id);
        Task<List<AppointmentDto>> GetAppointmentsByDoctorAsync(int doctorId);
        Task<List<AppointmentDto>> GetAppointmentsByStatusAsync(int statusId);
        Task<List<AppointmentDto>> GetAppointmentsByUserIdAsync(string userId);  
    }
}
