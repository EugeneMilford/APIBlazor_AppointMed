using AutoMapper;
using AppointMed.API.Data;
using AppointMed.API.Models.Doctor;
using AppointMed.API.Models.Appointment;
using AppointMed.API.Models.Status;
using AppointMed.API.Models.User;
using AppointMed.API.Models.Account;
using AppointMed.API.Models.Medicine;
using AppointMed.API.Models.Prescription;

namespace AppointMed.API.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            // Doctor mappings
            CreateMap<Doctor, DoctorDto>()
                .ForMember(dest => dest.ProfileImageUrl,
                    opt => opt.MapFrom(src => src.ProfileImageUrl != null ? src.ProfileImageUrl.ToString() : null))
                .ReverseMap()
                .ForMember(dest => dest.ProfileImageUrl,
                    opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.ProfileImageUrl) ? new Uri(src.ProfileImageUrl) : null));

            CreateMap<AddDoctorDto, Doctor>()
                .ForMember(dest => dest.ProfileImageUrl,
                    opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.ProfileImageUrl) ? new Uri(src.ProfileImageUrl) : null))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.DateJoined, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<UpdateDoctorDto, Doctor>()
                .ForMember(dest => dest.ProfileImageUrl,
                    opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.ProfileImageUrl) ? new Uri(src.ProfileImageUrl) : null))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now));

            CreateMap<Doctor, UpdateDoctorDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DoctorId))
                .ForMember(dest => dest.ProfileImageUrl,
                    opt => opt.MapFrom(src => src.ProfileImageUrl != null ? src.ProfileImageUrl.ToString() : null));

            // Appointment mappings 
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src =>
                    src.Doctor != null && !string.IsNullOrEmpty(src.Doctor.FirstName) && !string.IsNullOrEmpty(src.Doctor.LastName)
                        ? $"{src.Doctor.FirstName} {src.Doctor.LastName}"
                        : "No Doctor Assigned"))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src =>
                    src.Status != null && !string.IsNullOrEmpty(src.Status.StatusName)
                        ? src.Status.StatusName
                        : "Unknown"));

            CreateMap<AddAppointmentDto, Appointment>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.UserId, opt => opt.Ignore()); // UserId set in controller from JWT token

            CreateMap<UpdateAppointmentDto, Appointment>()
                .ForMember(dest => dest.AppointmentId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now));

            CreateMap<Appointment, UpdateAppointmentDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AppointmentId));

            // Status mappings
            CreateMap<Status, StatusDto>().ReverseMap();

            CreateMap<AddStatusDto, Status>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<UpdateStatusDto, Status>()
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.Id));

            CreateMap<Status, UpdateStatusDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.StatusId));

            // User mappings
            CreateMap<ApiUser, UserDto>().ReverseMap();

            // Account mappings
            CreateMap<Account, AccountDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src =>
                    src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : string.Empty));

            CreateMap<AccountTransaction, AccountTransactionDto>();

            // Medicine mappings
            CreateMap<Medicine, MedicineDto>();

            CreateMap<AddMedicineDto, Medicine>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.IsAvailable));

            CreateMap<UpdateMedicineDto, Medicine>()
                .ForMember(dest => dest.MedicineId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now));

            CreateMap<Medicine, UpdateMedicineDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.MedicineId));

            // Prescription mappings
            CreateMap<Data.Prescription, PrescriptionDto>()
                .ForMember(dest => dest.MedicineName, opt => opt.MapFrom(src =>
                    src.Medicine != null ? src.Medicine.Name : string.Empty))
                .ForMember(dest => dest.MedicinePrice, opt => opt.MapFrom(src =>
                    src.Medicine != null ? src.Medicine.Price : 0))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src =>
                    src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : string.Empty))
                .ForMember(dest => dest.TotalCost, opt => opt.MapFrom(src =>
                    src.Medicine != null ? src.Medicine.Price * src.Quantity : 0));

            CreateMap<AddPrescriptionDto, Data.Prescription>()
                .ForMember(dest => dest.PrescribedDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.IsFulfilled, opt => opt.MapFrom(src => false));

            CreateMap<Data.Prescription, AddPrescriptionDto>();
        }
    }
}