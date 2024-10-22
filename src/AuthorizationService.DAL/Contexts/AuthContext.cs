using System.Reflection;
using AuthorizationService.DAL.Extensions;
using AuthorizationService.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationService.DAL.Contexts;

public class AuthContext(DbContextOptions<AuthContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Contract> Contracts { get; set; }

    public DbSet<Passport> Passports { get; set; }

    public DbSet<WorkBook> WorkBooks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.SeedRole();
    }
}
