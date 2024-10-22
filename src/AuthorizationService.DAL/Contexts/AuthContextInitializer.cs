using AuthorizationService.DAL.Constants;
using AuthorizationService.DAL.Extensions;
using AuthorizationService.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthorizationService.DAL.Contexts;

public class AuthContextInitializer(
    ILogger<AuthContextInitializer> logger,
    AuthContext context,
    UserManager<User> userManager)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await context.Database.MigrateAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database.");

            throw;
        }
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await TrySeedAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");

            throw;
        }
    }

    private static User GetPreconfiguredAdmin()
    {
        return new User
        {
            Id = new Guid("1A24A4F4-E9CB-437F-9369-ED37448CA4C4"),
            Email = "admin@example.com",
            UserName = "admin@example.com",
            WorkBook = new WorkBook
            {
                Id = new Guid("94B9272E-93BB-41F1-ACE2-B5728109E3F5"),
                Number = "1234",
                IssueDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-1)),
            },
            Passport = new Passport
            {
                Id = new Guid("1E2BF72D-3F02-4A91-90B0-2A9050C0D1EA"),
                Firstname = "Ivan",
                Lastname = "Ivanov",
                Patronymic = "Ivanovich",
                Series = "AB",
                Number = "1234567",
                IdentificationNumber = "1234567a123PB1",
                BirthDate = DateOnly.ParseExact("2000-01-01", "yyyy-MM-dd"),
                IssueDate = DateOnly.ParseExact("2024-01-01", "yyyy-MM-dd"),
                ExpiryDate = DateOnly.ParseExact("2029-01-01", "yyyy-MM-dd"),
            },
            Contracts =
            [
                new Contract
                {
                    Id = new Guid("612913A5-B7AF-4E7A-B09D-AB3C96496FF8"),
                    Number = "1234",
                    StartDate = DateOnly.ParseExact("2024-01-01", "yyyy-MM-dd"),
                    EndDate = DateOnly.ParseExact("2026-01-01", "yyyy-MM-dd"),
                },
            ],
        };
    }

    private static void VerifyResult(IdentityResult? result)
    {
        if (result is { Succeeded: false })
        {
            throw new Exception(result.Errors.First().Description);
        }
    }

    private async Task TrySeedAsync(CancellationToken cancellationToken = default)
    {
        // Default administrator
        if (!userManager.Users.Any())
        {
            await SeedAdministratorAsync(cancellationToken);
        }
    }

    private async Task SeedAdministratorAsync(CancellationToken cancellationToken = default)
    {
        var admin = GetPreconfiguredAdmin();

        var result = await userManager.CreateAsync(admin, "Pass123$");
        VerifyResult(result);

        result = await userManager.AddToRoleAsync(admin, Roles.Administrator);
        VerifyResult(result);

        var claims = admin.ToClaims();
        claims.AddRange(admin.Passport!.ToClaims());
        result = await userManager.AddClaimsAsync(admin, claims);
        VerifyResult(result);
    }
}
