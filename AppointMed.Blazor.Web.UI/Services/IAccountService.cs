using AppointMed.Blazor.Web.UI.Services.Base;

namespace AppointMed.Blazor.Web.UI.Services
{
    public interface IAccountService
    {
        Task<Response<AccountDto>> GetMyAccount();
        Task<Response<List<AccountTransactionDto>>> GetMyTransactions();
    }
}