using LifeSyncApp.Client.Authentication;
using LifeSyncApp.Client.Services;
using LifeSyncApp.Client.Services.Contracts;
using LifeSyncApp.Client.Services.Http;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Syncfusion.Blazor;
using Syncfusion.Licensing;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Bind configuration for API base
var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
                  ?? builder.Configuration["Api:BaseUrl"]
                  ?? builder.Configuration["ApiGateway:BaseUrl"]
                  ?? builder.HostEnvironment.BaseAddress; // fallback to app origin

// Auth for WASM render mode
builder.Services.AddAuthorizationCore();
// Register CustomAuthStateProvider as both itself and as AuthenticationStateProvider (same instance)
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());

// Register a single HttpClient configured to call API Gateway
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// ApiClient facade
builder.Services.AddScoped<IApiClient, ApiClient>();

// App services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFinancialService, FinancialService>();
builder.Services.AddScoped<IGymService, GymService>();
builder.Services.AddScoped<INutritionService, NutritionService>();
builder.Services.AddScoped<ITaskManagerService, TaskManagerService>();

// Syncfusion
builder.Services.AddSyncfusionBlazor();
SyncfusionLicenseProvider.RegisterLicense(builder.Configuration["SyncfusionLicenseKey"]);

await builder.Build().RunAsync();
