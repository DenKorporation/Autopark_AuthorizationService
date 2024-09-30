using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthorizationService.DAL.Contexts;

public class PersistedGrantDbContextInitializer(
    ILogger<PersistedGrantDbContextInitializer> logger,
    PersistedGrantDbContext context)
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
}
