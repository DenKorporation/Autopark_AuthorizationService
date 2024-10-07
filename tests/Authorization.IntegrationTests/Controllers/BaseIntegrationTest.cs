using System.Text.Json;
using AuthorizationService.DAL.Contexts;
using AuthorizationService.DAL.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Authorization.IntegrationTests.Controllers;

public abstract class BaseIntegrationTest
    : IClassFixture<CustomWebApplicationFactory>,
        IDisposable
{
    private const string BasePath = "api/v1.0";
    private static string? _authenticationToken;

    private readonly CustomWebApplicationFactory _factory;
    private readonly IServiceScope _scope;

    protected readonly AuthContext TestDbContext;
    protected readonly HttpClient Client;
    protected readonly UserManager<User> UserManager;

    protected BaseIntegrationTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _scope = factory.Services.CreateScope();
        TestDbContext = _scope.ServiceProvider.GetRequiredService<AuthContext>();
        UserManager = _scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        Client = factory.CreateClient();

        if (_authenticationToken is null)
        {
            _authenticationToken = AuthenticateUserAsync(GetAdminCredential()).GetAwaiter().GetResult();
        }

        Client.SetBearerToken(_authenticationToken);

        if (Client.BaseAddress is not null)
        {
            Client.BaseAddress = new Uri(Client.BaseAddress.AbsoluteUri + BasePath);
        }
    }

    public void Dispose()
    {
        Client.Dispose();
        TestDbContext.Dispose();
        UserManager.Dispose();
        _scope.Dispose();
    }

    protected async Task AddEntitiesToDbAsync<T>(List<T> entities)
        where T : class
    {
        TestDbContext.AddRange(entities);
        await TestDbContext.SaveChangesAsync();
    }

    protected async Task AddEntitiesToDbAsync<T>(T entity)
        where T : class
    {
        TestDbContext.Add(entity);
        await TestDbContext.SaveChangesAsync();
    }

    private async Task<string> AuthenticateUserAsync(FormUrlEncodedContent parameters)
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsync("/connect/token", parameters);
        response.EnsureSuccessStatusCode();
        var jsonDocument = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var accessToken = jsonDocument.RootElement.GetProperty("access_token").GetString()!;

        return accessToken;
    }

    private FormUrlEncodedContent GetAdminCredential()
    {
        return new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "scope", "openid profile autopark offline_access" },
                { "username", "admin@example.com" },
                { "password", "Pass123$" },
                { "client_id", "test" },
            });
    }
}
