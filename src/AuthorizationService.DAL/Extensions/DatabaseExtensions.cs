using AuthorizationService.DAL.Constants;
using AuthorizationService.DAL.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AuthorizationService.DAL.Extensions;

public static class DatabaseExtensions
{
    public static async Task<IHost> InitializeDatabaseAsync(
        this IHost app,
        CancellationToken cancellationToken = default)
    {
        using var scope = app.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<AuthContextInitializer>();

        await initializer.InitializeAsync(cancellationToken);

        await initializer.SeedAsync(cancellationToken);

        return app;
    }

    public static async Task<IHost> InitializeOperationStoreAsync(
        this IHost app,
        CancellationToken cancellationToken = default)
    {
        using var scope = app.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContextInitializer>();

        await initializer.InitializeAsync(cancellationToken);

        return app;
    }

    public static ModelBuilder SeedRole(this ModelBuilder builder)
    {
        builder.Entity<IdentityRole<Guid>>().HasData(
            new IdentityRole<Guid>
            {
                Id = new Guid("6C093630-A093-491D-8475-F69CC8BFF29B"),
                Name = Roles.Administrator,
                NormalizedName = Roles.Administrator.ToUpper(),
            },
            new IdentityRole<Guid>
            {
                Id = new Guid("D5174FB7-6327-45B4-B443-EFB3888A786A"),
                Name = Roles.FleetManager,
                NormalizedName = Roles.FleetManager.ToUpper(),
            },
            new IdentityRole<Guid>
            {
                Id = new Guid("F18DB1FD-F479-43C6-A1AB-D343B00BC332"),
                Name = Roles.InsuranceAgent,
                NormalizedName = Roles.InsuranceAgent.ToUpper(),
            },
            new IdentityRole<Guid>
            {
                Id = new Guid("720E32DF-2964-4D96-85D4-1E527AAF839F"),
                Name = Roles.Technician,
                NormalizedName = Roles.Technician.ToUpper(),
            },
            new IdentityRole<Guid>
            {
                Id = new Guid("C338E6D7-F68E-495E-9A20-0B4158834207"),
                Name = Roles.HrManager,
                NormalizedName = Roles.HrManager.ToUpper(),
            },
            new IdentityRole<Guid>
            {
                Id = new Guid("8DBACE60-7185-410F-9F76-EB05BE65DC33"),
                Name = Roles.Driver,
                NormalizedName = Roles.Driver.ToUpper(),
            });

        return builder;
    }
}
