using AppointMed.Blazor.Web.UI.Services.Base;
using Blazored.LocalStorage;

namespace AppointMed.Blazor.Web.UI.Services
{
    public class PrescriptionService : BaseHttpService, IPrescriptionService
    {
        private readonly IClient client;

        public PrescriptionService(IClient client, ILocalStorageService localStorage)
            : base(client, localStorage)
        {
            this.client = client;
        }

        public async Task<Response<List<PrescriptionDto>>> GetAll()
        {
            Response<List<PrescriptionDto>> response;
            try
            {
                await GetBearerToken();
                var data = await client.PrescriptionsAllAsync();
                response = new Response<List<PrescriptionDto>>
                {
                    Data = data.ToList(),
                    Success = true
                };
            }
            catch (ApiException exception)
            {
                response = ConvertApiExceptions<List<PrescriptionDto>>(exception);
            }
            return response;
        }

        public async Task<Response<List<PrescriptionDto>>> GetMyPrescriptions()
        {
            Response<List<PrescriptionDto>> response;
            try
            {
                await GetBearerToken();
                var data = await client.MyPrescriptionsAsync();
                response = new Response<List<PrescriptionDto>>
                {
                    Data = data.ToList(),
                    Success = true
                };
            }
            catch (ApiException exception)
            {
                response = ConvertApiExceptions<List<PrescriptionDto>>(exception);
            }
            return response;
        }

        public async Task<Response<PrescriptionDto>> Get(int id)
        {
            Response<PrescriptionDto> response;
            try
            {
                await GetBearerToken();
                var data = await client.PrescriptionsGETAsync(id);
                response = new Response<PrescriptionDto>
                {
                    Data = data,
                    Success = true
                };
            }
            catch (ApiException exception)
            {
                response = ConvertApiExceptions<PrescriptionDto>(exception);
            }
            return response;
        }

        public async Task<Response<int>> Create(AddPrescriptionDto prescription)
        {
            Response<int> response;
            try
            {
                await GetBearerToken();
                await client.PrescriptionsPOSTAsync(prescription);
                response = new Response<int> { Success = true };
            }
            catch (ApiException exception)
            {
                response = ConvertApiExceptions<int>(exception);
            }
            return response;
        }

        public async Task<Response<int>> Fulfill(int id)
        {
            Response<int> response;
            try
            {
                await GetBearerToken();
                await client.FulfillAsync(id);
                response = new Response<int> { Success = true };
            }
            catch (ApiException exception)
            {
                response = ConvertApiExceptions<int>(exception);
            }
            return response;
        }

        public async Task<Response<int>> Delete(int id)
        {
            Response<int> response;
            try
            {
                await GetBearerToken();
                await client.PrescriptionsDELETEAsync(id);
                response = new Response<int> { Success = true };
            }
            catch (ApiException exception)
            {
                response = ConvertApiExceptions<int>(exception);
            }
            return response;
        }
    }
}