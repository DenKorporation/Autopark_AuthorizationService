using AuthorizationService.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthorizationService.DAL.Contexts.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .Ignore(u => u.PhoneNumber);
        builder
            .Ignore(u => u.PhoneNumberConfirmed);
        builder
            .Ignore(u => u.EmailConfirmed);
        builder
            .Ignore(u => u.TwoFactorEnabled);
        builder
            .Ignore(u => u.LockoutEnd);
        builder
            .Ignore(u => u.LockoutEnabled);

        builder
            .HasMany(u => u.Contracts)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId);

        builder
            .HasOne(u => u.Passport)
            .WithOne(p => p.User)
            .HasForeignKey<Passport>(p => p.UserId);

        builder
            .HasOne(u => u.WorkBook)
            .WithOne(wb => wb.User)
            .HasForeignKey<WorkBook>(wb => wb.UserId);

        builder
            .HasMany(u => u.Roles)
            .WithMany()
            .UsingEntity<IdentityUserRole<Guid>>();
    }
}
