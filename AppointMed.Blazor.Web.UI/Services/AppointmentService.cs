using AppointMed.Blazor.Web.UI.Services.Base;
using AutoMapper;
using Blazored.LocalStorage;

namespace AppointMed.Blazor.Web.UI.Services
{
    public class AppointmentService : BaseHttpService, IAppointmentService
    {
        private readonly IClient client;
        private readonly IMapper mapper;

        public AppointmentService(IClient client, ILocalStorageService localStorage, IMapper mapper) : base(client, localStorage)
        {
            this.client = client;
            this.mapper = mapper;
        }

        public async Task<Response<int>> Create(AddAppointmentDto appointment)
        {
            Response<int> response = new();

            try
            {
                await GetBearerToken();
                await client.AppointmentsPOSTAsync(appointment);
            }
            catch (ApiException exception)
            {
                response = ConvertApiExceptions<int>(exception);
            }

            return response;
        }

        public async Task<Response<int>> Delete(int id)
        {
            Response<int> response = new();

            try
            {
                await GetBearerToken();
                await client.AppointmentsDELETEAsync(id);
            }
            catch (ApiException exception)
            {
                response = ConvertApiExceptions<int>(exception);
            }

            return response;
        }

        public async Task<Response<int>> Edit(int id, UpdateAppointmentDto appointment)
        {
            Response<int> response = new();

            try
            {
                await GetBearerToken();
                await client.AppointmentsPUTAsync(id, appointment);
            }
            catch (ApiException exception)
            {
                response = ConvertApiExceptions<int>(exception);
            }

            return response;
        }

        public async Task<Response<AppointmentDto>> Get(int id)
        {
            Response<AppointmentDto> response;

            try
            {
                await GetBearerToken();
                var data = await client.AppointmentsGETAsync(id);
                response = new Response<AppointmentDto>
                {
                    Data = data,
                    Success = true
                };
            }
            catch (ApiException exception)
            {
                response = ConvertApiExceptions<AppointmentDto>(exception);
            }

            return response;
        }

        public async Task<Response<List<AppointmentDto>>> Get()
        {
            Response<List<AppointmentDto>> response;

            try
            {
                await GetBearerToken();
                var data = await client.AppointmentsAllAsync();
                response = new Response<List<AppointmentDto>>
                {
                    Data = data.ToList(),
                    Success = true
                };
            }
            catch (ApiException exception)
            {
                response = ConvertApiExceptions<List<AppointmentDto>>(exception);
            }

            return response;
        }

        public async Task<Response<UpdateAppointmentDto>> GetForUpdate(int id)
        {
            Response<UpdateAppointmentDto> response;

            try
            {
                await GetBearerToken();
                var data = await client.AppointmentsGETAsync(id);
                response = new Response<UpdateAppointmentDto>
                {
                    Data = mapper.Map<UpdateAppointmentDto>(data),
                    Success = true
                };
            }
            catch (ApiException exception)
            {
                response = ConvertApiExceptions<UpdateAppointmentDto>(exception);
            }

            return response;
        }
    }

}
