using AppointMed.Blazor.Web.UI.Providers;
using AppointMed.Blazor.Web.UI.Services.Base;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace AppointMed.Blazor.Web.UI.Services.Authentication
{
    public class AuthenticationService : BaseHttpService, IAuthenticationService
    {
        private readonly IClient httpClient;
        private readonly ILocalStorageService localStorage;
        private readonly AuthenticationStateProvider authenticationStateProvider;

        public AuthenticationService(IClient httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider)
            : base(httpClient, localStorage)
        {
            this.httpClient = httpClient;
            this.localStorage = localStorage;
            this.authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<Response<AuthResponse>> AuthenticateAsync(LoginUserDto loginModel)
        {
            Response<AuthResponse> response;
            try
            {
                var result = await httpClient.LoginAsync(loginModel);
                response = new Response<AuthResponse>
                {
                    Data = result,
                    Success = true,
                };

                try
                {
                    await localStorage.SetItemAsync("accessToken", result.Token);
                }
                catch (InvalidOperationException)
                {
                    
                }

                await ((ApiAuthenticationStateProvider)authenticationStateProvider).LoggedIn(result.Token);
            }
            catch (ApiException exception)
            {
                response = ConvertApiExceptions<AuthResponse>(exception);
            }

            return response;
        }

        public async Task Logout()
        {
            await ((ApiAuthenticationStateProvider)authenticationStateProvider).LoggedOut();
        }
    }
}
