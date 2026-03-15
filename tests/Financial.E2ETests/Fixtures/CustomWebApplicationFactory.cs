using Financial.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace Financial.E2ETests.Fixtures
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private PostgreSqlContainer? _postgresContainer;
        public string ConnectionString { get; private set; } = string.Empty;

        public async Task InitializeAsync()
        {
            _postgresContainer = new PostgreSqlBuilder()
                .WithImage("postgres:16")
                .WithDatabase("financial_e2e")
                .WithUsername("test")
                .WithPassword("test123")
                .WithCleanUp(true)
                .Build();

            await _postgresContainer.StartAsync();
            ConnectionString = _postgresContainer.GetConnectionString();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // Remove the existing DbContext registration
                services.RemoveAll<DbContextOptions<ApplicationDbContext>>();

                // Add DbContext with test container
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql(ConnectionString);
                });

                // Remove background services to avoid interference during tests
                services.RemoveAll<IHostedService>();

                // Re-add MigrationHostedService so migrations still run
                services.AddHostedService<MigrationHostedService>();

                // Configure test authentication
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "TestScheme";
                    options.DefaultChallengeScheme = "TestScheme";
                }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", _ => { });
            });

            builder.UseEnvironment("Testing");
        }

        public async Task ResetDatabaseAsync()
        {
            await using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = ["public"]
            });

            await respawner.ResetAsync(connection);
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            if (_postgresContainer != null)
            {
                await _postgresContainer.DisposeAsync();
            }
        }
    }
}
