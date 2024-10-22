using System.Data.Common;
using AuthorizationService.DAL.Contexts;
using DotNet.Testcontainers.Builders;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace Authorization.IntegrationTests;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>,
        IAsyncLifetime
{
    private const string TestingEnvironment = "Testing";

    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithDatabase("authorization")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithPortBinding(5432, true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("pg_isready"))
        .Build();

    private Respawner _respawner = null!;
    private DbConnection _dbConnection = null!;

    public async Task ResetDatabase()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        _dbConnection = new NpgsqlConnection(_postgresContainer.GetConnectionString());
        using var scope = Services.CreateScope();

        var authContextInitializer = scope.ServiceProvider.GetRequiredService<AuthContextInitializer>();
        await authContextInitializer.InitializeAsync();
        await authContextInitializer.SeedAsync();

        var persistedGrantDbContextInitializer =
            scope.ServiceProvider.GetRequiredService<PersistedGrantDbContextInitializer>();
        await persistedGrantDbContextInitializer.InitializeAsync();

        await InitializeRespawnerAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbConnection.DisposeAsync();
        await _postgresContainer.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(TestingEnvironment);
        builder.ConfigureTestServices(
            services =>
            {
                // AuthContext
                var authContextDescriptor = services
                    .SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<AuthContext>));

                if (authContextDescriptor is not null)
                {
                    services.Remove(authContextDescriptor);
                }

                services.AddDbContext<AuthContext>(
                    options =>
                        options.UseNpgsql(_postgresContainer.GetConnectionString()));

                // OperationalStore
                var operationalStoreDescriptor = services
                    .SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<PersistedGrantDbContext>));

                if (operationalStoreDescriptor is not null)
                {
                    services.Remove(operationalStoreDescriptor);
                }

                services.AddDbContext<PersistedGrantDbContext>(
                    options =>
                        options.UseNpgsql(
                            _postgresContainer.GetConnectionString(),
                            sql => sql.MigrationsAssembly(typeof(AuthContext).Assembly.GetName().Name)));
            });
    }

    private async Task InitializeRespawnerAsync()
    {
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(
            _dbConnection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                TablesToIgnore =
                [
                    "AspNetRoles",
                ],
                SchemasToInclude =
                [
                    "public",
                ],
            });
    }
}
