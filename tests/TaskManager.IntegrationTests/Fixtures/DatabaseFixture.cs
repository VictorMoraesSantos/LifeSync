using Microsoft.EntityFrameworkCore;
using Respawn;
using TaskManager.Infrastructure.Persistence.Data;
using Testcontainers.PostgreSql;

namespace TaskManager.IntegrationTests.Fixtures;

public class DatabaseFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _postgresContainer;
    public ApplicationDbContext DbContext { get; private set; } = null!;
    public string ConnectionString { get; private set; } = string.Empty;
    private Respawner? _respawner;

    public async Task InitializeAsync()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("taskmanager_test")
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

        _respawner = await Respawner.CreateAsync(ConnectionString,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = new[] { "public" }
            });
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
        if (_respawner != null)
        {
            await _respawner.ResetAsync(ConnectionString);
        }
    }
}
