using AppointMed.Blazor.Web.UI.Services.Base;
using AutoMapper;

namespace AppointMed.Blazor.Web.UI.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<DoctorDto, UpdateDoctorDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DoctorId))
                .ReverseMap();

            CreateMap<AppointmentDto, UpdateAppointmentDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AppointmentId))
                .ReverseMap();
        }
    }
}
