using AppointMed.API.Data;
using AppointMed.API.Models.Appointment;
using AppointMed.API.Repository.Interface;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AppointMed.API.Repository.Implementation
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        private readonly AppointMedDbContext context;
        private readonly IMapper mapper;

        public AppointmentRepository(AppointMedDbContext context, IMapper mapper) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<List<AppointmentDto>> GetAllAppointmentsAsync()
        {
            var appointments = await context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Status)
                .Include(a => a.User)
                .ToListAsync();

            return appointments.Select(a => MapToDto(a)).ToList();
        }

        public async Task<AppointmentDto> GetAppointmentAsync(int id)
        {
            var appointment = await context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Status)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            return appointment != null ? MapToDto(appointment) : null;
        }

        public async Task<List<AppointmentDto>> GetAppointmentsByDoctorAsync(int doctorId)
        {
            var appointments = await context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Status)
                .Include(a => a.User)
                .Where(a => a.DoctorId == doctorId)
                .ToListAsync();

            return appointments.Select(a => MapToDto(a)).ToList();
        }

        public async Task<List<AppointmentDto>> GetAppointmentsByStatusAsync(int statusId)
        {
            var appointments = await context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Status)
                .Include(a => a.User)
                .Where(a => a.StatusId == statusId)
                .ToListAsync();

            return appointments.Select(a => MapToDto(a)).ToList();
        }

        public async Task<List<AppointmentDto>> GetAppointmentsByUserIdAsync(string userId)
        {
            var appointments = await context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Status)
                .Include(a => a.User)
                .Where(a => a.UserId == userId)
                .ToListAsync();

            return appointments.Select(a => MapToDto(a)).ToList();
        }

        // Private helper method to map Appointment to AppointmentDto 
        private AppointmentDto MapToDto(Appointment appointment)
        {
            if (appointment == null)
                return null;

            return new AppointmentDto
            {
                AppointmentId = appointment.AppointmentId,
                UserId = appointment.UserId ?? string.Empty,
                PatientFirstName = appointment.PatientFirstName ?? string.Empty,
                PatientLastName = appointment.PatientLastName ?? string.Empty,
                PatientEmail = appointment.PatientEmail ?? string.Empty,
                PatientPhoneNumber = appointment.PatientPhoneNumber,
                AppointmentDateTime = appointment.AppointmentDateTime,
                AppointmentType = appointment.AppointmentType ?? string.Empty,
                Notes = appointment.Notes,
                DoctorId = appointment.DoctorId,
                DoctorName = GetDoctorName(appointment.Doctor),
                StatusId = appointment.StatusId,
                StatusName = GetStatusName(appointment.Status),
                CreatedAt = appointment.CreatedAt,
                UpdatedAt = appointment.UpdatedAt,
                CancelledAt = appointment.CancelledAt,
                CancellationReason = appointment.CancellationReason
            };
        }

        // Private helper method to get doctor name 
        private string GetDoctorName(Doctor doctor)
        {
            if (doctor == null)
                return "No Doctor Assigned";

            var firstName = doctor.FirstName ?? string.Empty;
            var lastName = doctor.LastName ?? string.Empty;
            var fullName = $"{firstName} {lastName}".Trim();

            return string.IsNullOrWhiteSpace(fullName) ? "Unknown Doctor" : fullName;
        }

        // Private helper method to get status name 
        private string GetStatusName(Status status)
        {
            if (status == null)
                return "Unknown";

            return status.StatusName ?? "Unknown";
        }
    }
}
