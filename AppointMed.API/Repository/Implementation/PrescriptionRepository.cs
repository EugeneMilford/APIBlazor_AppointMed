using AppointMed.API.Data;
using AppointMed.API.Models.Prescription;
using AppointMed.API.Repository.Interface;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AppointMed.API.Repository.Implementation
{
    public class PrescriptionRepository : GenericRepository<Prescription>, IPrescriptionRepository
    {
        private readonly AppointMedDbContext context;
        private readonly IMapper mapper;
        private readonly IAccountRepository accountRepository;

        public PrescriptionRepository(AppointMedDbContext context, IMapper mapper, IAccountRepository accountRepository) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.accountRepository = accountRepository;
        }

        public async Task<List<PrescriptionDto>> GetAllPrescriptionsAsync()
        {
            return await context.Prescriptions
                .Include(p => p.Medicine)
                .Include(p => p.User)
                .ProjectTo<PrescriptionDto>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<PrescriptionDto> GetPrescriptionDetailsAsync(int id)
        {
            return await context.Prescriptions
                .Include(p => p.Medicine)
                .Include(p => p.User)
                .ProjectTo<PrescriptionDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(p => p.PrescriptionId == id);
        }

        public async Task<List<PrescriptionDto>> GetPrescriptionsByUserIdAsync(string userId)
        {
            return await context.Prescriptions
                .Include(p => p.Medicine)
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .ProjectTo<PrescriptionDto>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<PrescriptionDto>> GetPrescriptionsByAppointmentIdAsync(int appointmentId)
        {
            return await context.Prescriptions
                .Include(p => p.Medicine)
                .Include(p => p.User)
                .Where(p => p.AppointmentId == appointmentId)
                .ProjectTo<PrescriptionDto>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task FulfillPrescriptionAsync(int prescriptionId, string userId)
        {
            var prescription = await context.Prescriptions
                .Include(p => p.Medicine)
                .FirstOrDefaultAsync(p => p.PrescriptionId == prescriptionId);

            if (prescription == null || prescription.IsFulfilled)
                return;

            prescription.IsFulfilled = true;
            prescription.FulfilledDate = DateTime.UtcNow;
            prescription.UpdatedAt = DateTime.UtcNow;

            // Get or create account
            var account = await accountRepository.GetOrCreateAccountAsync(userId);

            // Calculate total cost
            decimal totalCost = prescription.Medicine.Price * prescription.Quantity;

            // Add transaction to account
            await accountRepository.AddTransactionAsync(
                account.AccountId,
                "Prescription",
                totalCost,
                $"{prescription.Medicine.Name} x{prescription.Quantity}",
                prescriptionId: prescriptionId
            );

            await context.SaveChangesAsync();
        }
    }
}
