using AppointMed.API.Data;
using AppointMed.API.Models.Prescription;

namespace AppointMed.API.Repository.Interface
{
    public interface IPrescriptionRepository : IGenericRepository<Prescription>
    {
        Task<List<PrescriptionDto>> GetAllPrescriptionsAsync();
        Task<PrescriptionDto> GetPrescriptionDetailsAsync(int id);
        Task<List<PrescriptionDto>> GetPrescriptionsByUserIdAsync(string userId);
        Task<List<PrescriptionDto>> GetPrescriptionsByAppointmentIdAsync(int appointmentId);
        Task FulfillPrescriptionAsync(int prescriptionId, string userId);
    }
}