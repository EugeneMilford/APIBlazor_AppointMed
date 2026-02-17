using AppointMed.API.Data;
using AppointMed.API.Models.Medicine;
using AppointMed.API.Repository.Interface;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AppointMed.API.Repository.Implementation
{
    public class MedicineRepository : GenericRepository<Medicine>, IMedicineRepository
    {
        private readonly AppointMedDbContext context;
        private readonly IMapper mapper;

        public MedicineRepository(AppointMedDbContext context, IMapper mapper) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<List<MedicineDto>> GetAllMedicinesAsync()
        {
            return await context.Medicines
                .ProjectTo<MedicineDto>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<MedicineDto> GetMedicineDetailsAsync(int id)
        {
            return await context.Medicines
                .ProjectTo<MedicineDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(m => m.MedicineId == id);
        }

        public async Task<List<MedicineDto>> GetAvailableMedicinesAsync()
        {
            return await context.Medicines
                .Where(m => m.IsAvailable)
                .ProjectTo<MedicineDto>(mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}
