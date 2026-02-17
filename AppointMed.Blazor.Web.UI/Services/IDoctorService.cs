using AppointMed.Blazor.Web.UI.Services.Base;

namespace AppointMed.Blazor.Web.UI.Services
{
    public interface IDoctorService
    {
        Task<Response<List<DoctorDto>>> Get();
        Task<Response<DoctorDto>> Get(int id);
        Task<Response<UpdateDoctorDto>> GetForUpdate(int id);
        Task<Response<int>> Create(AddDoctorDto doctor);
        Task<Response<int>> Edit(int id, UpdateDoctorDto doctor);
        Task<Response<int>> Delete(int id);
    }
}
