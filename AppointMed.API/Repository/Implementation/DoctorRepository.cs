using AppointMed.API.Data;
using AppointMed.API.Models.Doctor;
using AppointMed.API.Repository.Interface;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AppointMed.API.Repository.Implementation
{
    public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
    {
        private readonly AppointMedDbContext context;
        private readonly IMapper mapper;

        public DoctorRepository(AppointMedDbContext context, IMapper mapper) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<List<DoctorDto>> GetAllDoctorsAsync()
        {
            return await context.Doctors
                .ProjectTo<DoctorDto>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<DoctorDto> GetDoctorDetailsAsync(int id)
        {
            return await context.Doctors
                .ProjectTo<DoctorDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(q => q.DoctorId == id);
        }
    }
}
