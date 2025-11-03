using LifeSyncApp.Client.Services;
using LifeSyncApp.Components;
using LifeSyncApp.Services;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// Base address for API calls (match the WASM client)
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:6006";

// Provide HttpClient for prerendering/server DI
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// Register server-safe storage and auth for prerendering
builder.Services.AddScoped<ILocalStorageService, ServerLocalStorageService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Register Syncfusion services for possible prerendering or server rendering
builder.Services.AddSyncfusionBlazor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(LifeSyncApp.Client._Imports).Assembly);

app.Run();
