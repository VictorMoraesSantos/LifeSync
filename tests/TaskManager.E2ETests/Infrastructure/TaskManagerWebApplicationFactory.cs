using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using TaskManager.Infrastructure.Persistence.Data;
using Testcontainers.PostgreSql;

namespace TaskManager.E2ETests.Infrastructure;

public class TaskManagerWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithDatabase("taskmanager_e2e")
        .WithUsername("test")
        .WithPassword("test123")
        .WithCleanUp(true)
        .Build();

    public string ConnectionString => _dbContainer.GetConnectionString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _dbContainer.GetConnectionString()
            });
        });

        builder.ConfigureServices(services =>
        {
            // Remove background services that interfere with tests
            RemoveHostedServiceDescriptor(services, "DueDateReminderService");
            RemoveHostedServiceDescriptor(services, "MigrationHostedService");

            // Override authentication to allow unauthenticated test requests
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
        });

        builder.UseEnvironment("Testing");
    }

    private static void RemoveHostedServiceDescriptor(IServiceCollection services, string typeName)
    {
        var descriptor = services.FirstOrDefault(d =>
            d.ImplementationType?.Name == typeName ||
            d.ImplementationFactory?.Method.Name.Contains(typeName) == true);

        if (descriptor != null)
            services.Remove(descriptor);
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        // Run migrations
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _dbContainer.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.TaskItems.RemoveRange(dbContext.TaskItems);
        dbContext.TaskLabels.RemoveRange(dbContext.TaskLabels);
        await dbContext.SaveChangesAsync();
    }
}

/// <summary>
/// Test authentication handler that automatically authenticates all requests.
/// </summary>
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "Test User"),
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Email, "test@test.com")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
