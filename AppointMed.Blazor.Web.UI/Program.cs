using AppointMed.Blazor.Web.UI.Components;
using AppointMed.Blazor.Web.UI.Configurations;
using AppointMed.Blazor.Web.UI.Providers;
using AppointMed.Blazor.Web.UI.Services;
using AppointMed.Blazor.Web.UI.Services.Authentication;
using AppointMed.Blazor.Web.UI.Services.Base;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options =>
    {
        if (builder.Environment.IsDevelopment())
        {
            options.DetailedErrors = true;
        }
    });

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddAuthorizationCore();

builder.Services.AddHttpClient();

// Register Client manually
builder.Services.AddScoped<IClient>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient();
    httpClient.BaseAddress = new Uri("https://localhost:7017/");
    return new Client("https://localhost:7017/", httpClient);
});

// Adding AutoMapper
builder.Services.AddAutoMapper(typeof(MapperConfig));

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IAccountService, AccountService>();          
builder.Services.AddScoped<IMedicineService, MedicineService>();         
builder.Services.AddScoped<IPrescriptionService, PrescriptionService>(); 

// Registering the Authentication State Provider
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(p =>
    p.GetRequiredService<ApiAuthenticationStateProvider>());

var app = builder.Build();
app.UseDeveloperExceptionPage();

// Configuring the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();