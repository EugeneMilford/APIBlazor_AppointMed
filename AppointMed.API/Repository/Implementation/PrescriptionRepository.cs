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

        public async Task<PrescriptionDto> GetPrescriptionAsync(int id)
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

        public async Task FulfillPrescriptionAsync(int id)
        {
            var prescription = await context.Prescriptions
                .Include(p => p.Medicine)
                .FirstOrDefaultAsync(p => p.PrescriptionId == id);

            if (prescription == null)
                throw new InvalidOperationException("Prescription not found");

            if (prescription.IsFulfilled)
                throw new InvalidOperationException("Prescription is already fulfilled");

            // Mark as fulfilled
            prescription.IsFulfilled = true;
            prescription.FulfilledDate = DateTime.UtcNow;

            // Get or create account
            var account = await accountRepository.GetOrCreateAccountAsync(prescription.UserId);

            // Calculate total cost
            decimal totalCost = (decimal)(prescription.Medicine.Price * prescription.Quantity);

            // Add transaction to account
            await accountRepository.AddTransactionAsync(
                accountId: account.AccountId,
                transactionType: "Prescription",
                amount: totalCost,
                description: $"Prescription for {prescription.Medicine.Name} (x{prescription.Quantity})",
                appointmentId: null,
                prescriptionId: prescription.PrescriptionId
            );

            await context.SaveChangesAsync();
        }

        public async Task<List<Prescription>> GetPrescriptionEntitiesByAppointmentIdAsync(int appointmentId)
        {
            return await context.Prescriptions
                .Where(p => p.AppointmentId == appointmentId)
                .ToListAsync();
        }
    }
}