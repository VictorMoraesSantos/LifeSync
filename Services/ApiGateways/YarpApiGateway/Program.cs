using Microsoft.AspNetCore.DataProtection;
using YarpApiGateway;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthenticationService(builder.Configuration);

// Persist Data Protection keys to disk to avoid ephemeral in-memory keys (important for containers)
var dataProtectionKeyPath = builder.Environment.IsDevelopment()
 ? Path.Combine(builder.Environment.ContentRootPath, "dp-keys")
 : "/keys"; // mount this path as a volume in Docker

builder.Services
 .AddDataProtection()
 .SetApplicationName("LifeSync")
 .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeyPath));

// ? Adicionar CORS com todas as origens necessárias
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins(
            "http://localhost:6007",
            "https://localhost:6067")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// ? Usar CORS ANTES de Authentication/Authorization
app.UseCors("AllowBlazorApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.Run();
