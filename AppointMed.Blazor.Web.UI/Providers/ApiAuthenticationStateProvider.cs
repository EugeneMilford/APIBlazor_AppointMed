using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AppointMed.Blazor.Web.UI.Providers
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService localStorage;
        private readonly JwtSecurityTokenHandler jwtSecurityTokenHandler;

        public ApiAuthenticationStateProvider(ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
            jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity());

            try
            {
                var savedToken = await localStorage.GetItemAsync<string>("accessToken");

                if (string.IsNullOrWhiteSpace(savedToken))
                {
                    return new AuthenticationState(user);
                }

                var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(savedToken);

                if (tokenContent.ValidTo < DateTime.Now)
                {
                    await localStorage.RemoveItemAsync("accessToken");
                    return new AuthenticationState(user);
                }

                var claims = GetClaimsFromToken(savedToken);
                user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));

                return new AuthenticationState(user);
            }
            catch (InvalidOperationException)
            {
                return new AuthenticationState(user);
            }
        }

        public async Task LoggedIn(string token)
        {
            var claims = GetClaimsFromToken(token);
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
            var authState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);
        }

        public async Task LoggedOut()
        {
            try
            {
                await localStorage.RemoveItemAsync("accessToken");
            }
            catch (InvalidOperationException)
            {
                // Swallow exception during prerender
            }

            var nobody = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(nobody));
            NotifyAuthenticationStateChanged(authState);
        }

        private List<Claim> GetClaimsFromToken(string token)
        {
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(token);
            var claims = tokenContent.Claims.ToList();
            claims.Add(new Claim(ClaimTypes.Name, tokenContent.Subject));
            return claims;
        }
    }
}