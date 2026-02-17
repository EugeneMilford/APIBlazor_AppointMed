using AppointMed.Blazor.Web.UI.Services.Base;
using Blazored.LocalStorage;

namespace AppointMed.Blazor.Web.UI.Services
{
    public class MedicineService : BaseHttpService, IMedicineService
    {
        private readonly IClient client;

        public MedicineService(IClient client, ILocalStorageService localStorage)
            : base(client, localStorage)
        {
            this.client = client;
        }

        public async Task<Response<List<MedicineDto>>> Get()
        {
            Response<List<MedicineDto>> response;
            try
            {
                await GetBearerToken();
                var data = await client.MedicinesAllAsync();
                response = new Response<List<MedicineDto>>
                {
                    Data = data.ToList(),
                    Success = true
                };
            }
            catch (ApiException exception)
            {
                response = ConvertApiExceptions<List<MedicineDto>>(exception);
            }
            return response;
        }

        public async Task<Response<List<MedicineDto>>> GetAvailable()
        {
            Response<List<MedicineDto>> response;
            try
            {
                await GetBearerToken();
                var data = await client.AvailableAsync();
                response = new Response<List<MedicineDto>>
                {
                    Data = data.ToList(),
                    Success = true
                };
            }
            catch (ApiException exception)
            {
                response = ConvertApiExceptions<List<MedicineDto>>(exception);
            }
            return response;
        }

        public async Task<Response<MedicineDto>> Get(int id)
        {
            Response<MedicineDto> response;
            try
            {
                await GetBearerToken();
                var data = await client.MedicinesGETAsync(id);
                response = new Response<MedicineDto>
                {
                    Data = data,
                    Success = true
                };
            }
            catch (ApiException exception)
            {
                response = ConvertApiExceptions<MedicineDto>(exception);
            }
            return response;
        }

        public async Task<Response<int>> Create(AddMedicineDto medicine)
        {
            Response<int> response;
            try
            {
                await GetBearerToken();
                await client.MedicinesPOSTAsync(medicine);
                response = new Response<int> { Success = true };
            }
            catch (ApiException exception)
            {
                response = ConvertApiExceptions<int>(exception);
            }
            return response;
        }

        public async Task<Response<int>> Edit(int id, UpdateMedicineDto medicine)
        {
            Response<int> response;
            try
            {
                await GetBearerToken();
                await client.MedicinesPUTAsync(id, medicine);
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
                await client.MedicinesDELETEAsync(id);
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
