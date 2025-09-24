using YarpApiGateway;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthenticationService(builder.Configuration);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.Run();
