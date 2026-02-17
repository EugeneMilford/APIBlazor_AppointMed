using AppointMed.Blazor.Web.UI.Services.Base;

namespace AppointMed.Blazor.Web.UI.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<Response<AuthResponse>> AuthenticateAsync(LoginUserDto loginModel);
        public Task Logout();
    }
}
