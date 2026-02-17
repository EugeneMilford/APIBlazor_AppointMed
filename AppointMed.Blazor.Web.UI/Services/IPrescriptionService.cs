using AppointMed.Blazor.Web.UI.Services.Base;
using System.Runtime.Intrinsics.X86;

namespace AppointMed.Blazor.Web.UI.Services
{
    public interface IPrescriptionService
    {
        Task<Response<List<PrescriptionDto>>> GetAll();
        Task<Response<List<PrescriptionDto>>> GetMyPrescriptions();
        Task<Response<PrescriptionDto>> Get(int id);
        Task<Response<int>> Create(AddPrescriptionDto prescription);
        Task<Response<int>> Fulfill(int id);
        Task<Response<int>> Delete(int id);
    }
}
