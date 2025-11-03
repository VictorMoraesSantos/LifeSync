using YarpApiGateway;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthenticationService(builder.Configuration);

// ? Adicionar CORS com todas as origens necessárias
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins(
       "http://localhost:6007",   // HTTP do WebApp
    "https://localhost:6067",  // HTTPS do WebApp
               "http://localhost:5068",   // HTTP alternativo
       "https://localhost:7124"   // HTTPS alternativo
           )
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
