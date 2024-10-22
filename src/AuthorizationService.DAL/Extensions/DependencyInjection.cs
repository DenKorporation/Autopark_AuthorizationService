using System.Reflection;
using System.Security.Cryptography;
using AuthorizationService.DAL.Contexts;
using AuthorizationService.DAL.Models;
using AuthorizationService.DAL.Repositories.Implementations;
using AuthorizationService.DAL.Repositories.Interfaces;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AuthorizationService.DAL.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddDalServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.ConfigureDbContext(configuration);
        services.ConfigureIdentity();
        services.ConfigureIdentityServer(configuration);
        services.ConfigureRepositories();

        return services;
    }

    private static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthContext>(
            options =>
                options.UseNpgsql(configuration.GetConnectionString("Database")));

        services.AddScoped<AuthContextInitializer>();
    }

    private static void ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AuthContext>();
    }

    private static IServiceCollection ConfigureIdentityServer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var identityResources = configuration.GetSection("IdentityServer:IdentityResources")
                                    .Get<IEnumerable<IdentityResource>>() ??
                                throw new InvalidOperationException(
                                    "Incorrect configuration for 'IdentityServer:IdentityResources'");
        var apiScopes = configuration.GetSection("IdentityServer:ApiScopes").Get<IEnumerable<ApiScope>>() ??
                        throw new InvalidOperationException("Incorrect configuration for 'IdentityServer:ApiScopes'");
        var clients = configuration.GetSection("IdentityServer:Clients").Get<IEnumerable<Client>>() ??
                      throw new InvalidOperationException("Incorrect configuration for 'IdentityServer:Clients'");

        services
            .AddIdentityServer()
            .AddOperationalStore(
                options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseNpgsql(
                            configuration.GetConnectionString("Database"),
                            sql => sql.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));

                    options.DefaultSchema = "IdentityServer";
                    options.EnableTokenCleanup = true;
                })
            .AddSigningCredential(
                GetRsaSigningCredentials(configuration.GetRequiredSection("Identity:PrivateKey").Value!))
            .AddInMemoryIdentityResources(identityResources)
            .AddInMemoryApiScopes(apiScopes)
            .AddInMemoryClients(clients)
            .AddAspNetIdentity<User>();

        services.AddScoped<PersistedGrantDbContextInitializer>();

        return services;
    }

    private static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPassportRepository, PassportRepository>();
        services.AddScoped<IContractRepository, ContractRepository>();
        services.AddScoped<IWorkBookRepository, WorkBookRepository>();
        services.AddScoped<IUserClaimRepository, UserClaimRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static SigningCredentials GetRsaSigningCredentials(string privateKeyBase64)
    {
        var rsa = RSA.Create();

        var privateKeyBytes = Convert.FromBase64String(privateKeyBase64);
        rsa.ImportRSAPrivateKey(privateKeyBytes, out _);

        return new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
    }
}
