using AppointMed.API.Data;
using AppointMed.API.Models.Account;
using AppointMed.API.Repository.Interface;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AppointMed.API.Repository.Implementation
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        private readonly AppointMedDbContext context;
        private readonly IMapper mapper;

        public AccountRepository(AppointMedDbContext context, IMapper mapper) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<AccountDto> GetAccountByUserIdAsync(string userId)
        {
            return await context.Accounts
                .Include(a => a.User)
                .Where(a => a.UserId == userId)
                .ProjectTo<AccountDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<Account> GetOrCreateAccountAsync(string userId)
        {
            var account = await context.Accounts
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
            {
                account = new Account
                {
                    UserId = userId,
                    Balance = 0,
                    CreatedAt = DateTime.UtcNow
                };

                context.Accounts.Add(account);
                await context.SaveChangesAsync();
            }

            return account;
        }

        public async Task<List<AccountTransactionDto>> GetTransactionsByAccountIdAsync(int accountId)
        {
            return await context.AccountTransactions
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.TransactionDate)
                .ProjectTo<AccountTransactionDto>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task AddTransactionAsync(int accountId, string transactionType, decimal amount, string description, int? appointmentId = null, int? prescriptionId = null)
        {
            var transaction = new AccountTransaction
            {
                AccountId = accountId,
                TransactionType = transactionType,
                Amount = amount,
                Description = description,
                AppointmentId = appointmentId,
                PrescriptionId = prescriptionId,
                TransactionDate = DateTime.UtcNow
            };

            context.AccountTransactions.Add(transaction);

            // Update account balance
            var account = await context.Accounts.FindAsync(accountId);
            if (account != null)
            {
                account.Balance += amount;
                account.UpdatedAt = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();
        }

        public async Task RefundTransactionAsync(int accountId, int appointmentId)
        {
            var account = await context.Accounts.FindAsync(accountId);
            if (account == null)
            {
                throw new InvalidOperationException($"Account {accountId} not found");
            }

            // Find the original transaction for this appointment
            var originalTransaction = await context.AccountTransactions
                .FirstOrDefaultAsync(t => t.AccountId == accountId && t.AppointmentId == appointmentId);

            if (originalTransaction == null)
            {
                Console.WriteLine($"No transaction found for appointment {appointmentId} in account {accountId}");
                return; // No transaction to refund
            }

            Console.WriteLine($"Refunding appointment transaction {originalTransaction.TransactionId} with amount {originalTransaction.Amount}");

            // Deduct the amount from account balance (refund)
            account.Balance -= originalTransaction.Amount;
            account.UpdatedAt = DateTime.UtcNow;

            // Create a reversal/refund transaction
            var refundTransaction = new AccountTransaction
            {
                AccountId = accountId,
                TransactionType = "Refund",
                Amount = -originalTransaction.Amount,
                Description = $"Refund for cancelled appointment #{appointmentId}",
                AppointmentId = appointmentId,
                TransactionDate = DateTime.UtcNow
            };

            context.AccountTransactions.Add(refundTransaction);
            await context.SaveChangesAsync();
        }

        public async Task RefundPrescriptionAsync(int accountId, int prescriptionId)
        {
            var account = await context.Accounts.FindAsync(accountId);
            if (account == null)
            {
                throw new InvalidOperationException($"Account {accountId} not found");
            }

            // Find the original transaction for this prescription
            var originalTransaction = await context.AccountTransactions
                .FirstOrDefaultAsync(t => t.AccountId == accountId && t.PrescriptionId == prescriptionId);

            if (originalTransaction == null)
            {
                Console.WriteLine($"No transaction found for prescription {prescriptionId} in account {accountId}");
                return; // No transaction to refund
            }

            Console.WriteLine($"Refunding prescription transaction {originalTransaction.TransactionId} with amount {originalTransaction.Amount}");

            // Deduct the amount from account balance (refund)
            account.Balance -= originalTransaction.Amount;
            account.UpdatedAt = DateTime.UtcNow;

            // Create a reversal/refund transaction
            var refundTransaction = new AccountTransaction
            {
                AccountId = accountId,
                TransactionType = "Refund",
                Amount = -originalTransaction.Amount,
                Description = $"Refund for cancelled prescription #{prescriptionId}",
                PrescriptionId = prescriptionId,
                TransactionDate = DateTime.UtcNow
            };

            context.AccountTransactions.Add(refundTransaction);
            await context.SaveChangesAsync();
        }
    }
}