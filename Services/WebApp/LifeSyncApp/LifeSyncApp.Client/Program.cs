using LifeSyncApp.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Syncfusion.Blazor;
using Syncfusion.Licensing;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Configure a URL do API Gateway corretamente
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5006";

// Register LocalStorage first
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();

// Register HttpClient with authentication handler
builder.Services.AddScoped<AuthHttpMessageHandler>();
builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<AuthHttpMessageHandler>();
    handler.InnerHandler = new HttpClientHandler();
    return new HttpClient(handler) { BaseAddress = new Uri(apiBaseUrl) };
});

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITaskManagerService, TaskManagerService>();
builder.Services.AddScoped<INutritionService, NutritionService>();
builder.Services.AddScoped<IFinancialService, FinancialService>();
builder.Services.AddScoped<IGymService, GymService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Syncfusion Blazor services
builder.Services.AddSyncfusionBlazor();
SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JFaF5cXGRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWH5ec3VVRWdZUUxyW0JWYEg=");

await builder.Build().RunAsync();
