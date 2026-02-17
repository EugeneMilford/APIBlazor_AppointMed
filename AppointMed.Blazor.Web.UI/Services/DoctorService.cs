using AppointMed.Blazor.Web.UI.Services.Base;
using AutoMapper;
using Blazored.LocalStorage;

namespace AppointMed.Blazor.Web.UI.Services
{
    public class DoctorService : BaseHttpService, IDoctorService
    {
        private readonly IClient client;
        private readonly IMapper mapper;

        public DoctorService(IClient client, ILocalStorageService localStorage, IMapper mapper)
            : base(client, localStorage)
        {
            this.client = client;
            this.mapper = mapper;
        }

        public async Task<Response<List<DoctorDto>>> Get()
        {
            Response<List<DoctorDto>> response;

            try
            {
                await GetBearerToken();
                var data = await client.DoctorsAllAsync();
                response = new Response<List<DoctorDto>>
                {
                    Data = data.ToList(),
                    Success = true
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<List<DoctorDto>>(ex);
            }

            return response;
        }

        public async Task<Response<DoctorDto>> Get(int id)
        {
            Response<DoctorDto> response;

            try
            {
                await GetBearerToken();
                var data = await client.DoctorsGETAsync(id);
                response = new Response<DoctorDto>
                {
                    Data = data,
                    Success = true
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<DoctorDto>(ex);
            }

            return response;
        }

        public async Task<Response<UpdateDoctorDto>> GetForUpdate(int id)
        {
            Response<UpdateDoctorDto> response;

            try
            {
                await GetBearerToken();
                var data = await client.DoctorsGETAsync(id);
                var mappedData = mapper.Map<UpdateDoctorDto>(data);
                response = new Response<UpdateDoctorDto>
                {
                    Data = mappedData,
                    Success = true
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<UpdateDoctorDto>(ex);
            }

            return response;
        }

        public async Task<Response<int>> Create(AddDoctorDto doctor)
        {
            Response<int> response;

            try
            {
                await GetBearerToken();
                await client.DoctorsPOSTAsync(doctor);
                response = new Response<int> { Success = true };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<int>(ex);
            }

            return response;
        }

        public async Task<Response<int>> Edit(int id, UpdateDoctorDto doctor)
        {
            Response<int> response;

            try
            {
                await GetBearerToken();
                await client.DoctorsPUTAsync(id, doctor);
                response = new Response<int> { Success = true };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<int>(ex);
            }

            return response;
        }

        public async Task<Response<int>> Delete(int id)
        {
            Response<int> response;

            try
            {
                await GetBearerToken();
                await client.DoctorsDELETEAsync(id);
                response = new Response<int> { Success = true };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<int>(ex);
            }

            return response;
        }
    }
}