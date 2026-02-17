using AppointMed.Blazor.Web.UI.Services.Base;

namespace AppointMed.Blazor.Web.UI.Services
{
    public interface IMedicineService
    {
        Task<Response<List<MedicineDto>>> Get();
        Task<Response<List<MedicineDto>>> GetAvailable();
        Task<Response<MedicineDto>> Get(int id);
        Task<Response<int>> Create(AddMedicineDto medicine);
        Task<Response<int>> Edit(int id, UpdateMedicineDto medicine);
        Task<Response<int>> Delete(int id);
    }
}