using AppointMed.Blazor.Web.UI.Services.Base;
using Blazored.LocalStorage;

namespace AppointMed.Blazor.Web.UI.Services
{
    public class AccountService : BaseHttpService, IAccountService
    {
        private readonly IClient client;

        public AccountService(IClient client, ILocalStorageService localStorage)
            : base(client, localStorage)
        {
            this.client = client;
        }

        public async Task<Response<AccountDto>> GetMyAccount()
        {
            Response<AccountDto> response;
            try
            {
                await GetBearerToken();
                var data = await client.MyAccountAsync();
                response = new Response<AccountDto>
                {
                    Data = data,
                    Success = true
                };
            }
            catch (ApiException exception)
            {
                response = ConvertApiExceptions<AccountDto>(exception);
            }
            return response;
        }

        public async Task<Response<List<AccountTransactionDto>>> GetMyTransactions()
        {
            Response<List<AccountTransactionDto>> response;
            try
            {
                await GetBearerToken();
                var data = await client.MyTransactionsAsync();
                response = new Response<List<AccountTransactionDto>>
                {
                    Data = data.ToList(),
                    Success = true
                };
            }
            catch (ApiException exception)
            {
                response = ConvertApiExceptions<List<AccountTransactionDto>>(exception);
            }
            return response;
        }
    }
}