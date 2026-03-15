using Microsoft.EntityFrameworkCore;
using Npgsql;
using Nutrition.Infrastructure.Persistence.Data;
using Respawn;
using Testcontainers.PostgreSql;

namespace Nutrition.IntegrationTests.Fixtures
{
    public class DatabaseFixture : IAsyncLifetime
    {
        private PostgreSqlContainer? _postgresContainer;
        public ApplicationDbContext DbContext { get; private set; } = null!;
        public string ConnectionString { get; private set; } = string.Empty;

        public async Task InitializeAsync()
        {
            _postgresContainer = new PostgreSqlBuilder()
                .WithImage("postgres:16")
                .WithDatabase("nutrition_test")
                .WithUsername("test")
                .WithPassword("test123")
                .WithCleanUp(true)
                .Build();

            await _postgresContainer.StartAsync();

            ConnectionString = _postgresContainer.GetConnectionString();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(ConnectionString)
                .Options;

            DbContext = new ApplicationDbContext(options);

            await DbContext.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            await DbContext.DisposeAsync();
            if (_postgresContainer != null)
            {
                await _postgresContainer.DisposeAsync();
            }
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

        public ApplicationDbContext CreateNewContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(ConnectionString)
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
