using AppointMed.API.Data;
using AppointMed.API.Models.Prescription;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace AppointMed.API.Repository.Interface
{
    public interface IPrescriptionRepository : IGenericRepository<Prescription>
    {
        Task<List<PrescriptionDto>> GetAllPrescriptionsAsync();
        Task<List<PrescriptionDto>> GetPrescriptionsByUserIdAsync(string userId);
        Task<PrescriptionDto> GetPrescriptionAsync(int id);
        Task<List<PrescriptionDto>> GetPrescriptionsByAppointmentIdAsync(int appointmentId);
        Task FulfillPrescriptionAsync(int id);
        Task<List<Prescription>> GetPrescriptionEntitiesByAppointmentIdAsync(int appointmentId);
    }
}
