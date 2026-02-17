using AppointMed.API.Data;
using AppointMed.API.Models.Account;

namespace AppointMed.API.Repository.Interface
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<AccountDto> GetAccountByUserIdAsync(string userId);
        Task<Account> GetOrCreateAccountAsync(string userId);
        Task<List<AccountTransactionDto>> GetTransactionsByAccountIdAsync(int accountId);
        Task AddTransactionAsync(int accountId, string transactionType, decimal amount, string description, int? appointmentId = null, int? prescriptionId = null);
        Task RefundTransactionAsync(int accountId, int appointmentId); 
    }
}
