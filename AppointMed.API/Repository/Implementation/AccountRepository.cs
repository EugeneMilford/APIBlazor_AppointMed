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

            // Updating account balance
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

            // First, try to find transaction by appointmentId
            var originalTransaction = await context.AccountTransactions
                .FirstOrDefaultAsync(t => t.AccountId == accountId && t.AppointmentId == appointmentId);

            // If not found, try to find by description (fallback for old records)
            if (originalTransaction == null)
            {
                // Get the appointment details to match description
                var appointment = await context.Appointments.FindAsync(appointmentId);
                if (appointment != null)
                {
                    // Try to find transaction by matching description pattern
                    var transactions = await context.AccountTransactions
                        .Where(t => t.AccountId == accountId &&
                                   t.TransactionType == "Appointment" &&
                                   t.AppointmentId == null &&
                                   t.Amount == 200)
                        .OrderByDescending(t => t.TransactionDate)
                        .ToListAsync();

                    // Take the most recent unlinked appointment transaction
                    originalTransaction = transactions.FirstOrDefault();
                }
            }

            if (originalTransaction == null)
            {
                Console.WriteLine($"No transaction found for appointment {appointmentId} in account {accountId}");
                return; // No transaction to refund
            }

            Console.WriteLine($"Refunding transaction {originalTransaction.TransactionId} with amount {originalTransaction.Amount}");

            // Deduct the amount from account balance (refund)
            account.Balance -= originalTransaction.Amount;
            account.UpdatedAt = System.DateTime.UtcNow;

            // Creating a reversal/refund transaction
            var refundTransaction = new AccountTransaction
            {
                AccountId = accountId,
                TransactionType = "Refund",
                Amount = -originalTransaction.Amount,
                Description = $"Refund for cancelled appointment #{appointmentId}",
                AppointmentId = appointmentId,
                TransactionDate = System.DateTime.UtcNow
            };

            context.AccountTransactions.Add(refundTransaction);
            await context.SaveChangesAsync();
        }
    }
}