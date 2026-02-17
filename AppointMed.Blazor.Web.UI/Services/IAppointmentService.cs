using AppointMed.Blazor.Web.UI.Services.Base;

namespace AppointMed.Blazor.Web.UI.Services
{
    public interface IAppointmentService
    {
        Task<Response<List<AppointmentDto>>> Get();
        Task<Response<AppointmentDto>> Get(int id);
        Task<Response<UpdateAppointmentDto>> GetForUpdate(int id);
        Task<Response<int>> Create(AddAppointmentDto appointment);
        Task<Response<int>> Edit(int id, UpdateAppointmentDto appointment);
        Task<Response<int>> Delete(int id);
    }

}
