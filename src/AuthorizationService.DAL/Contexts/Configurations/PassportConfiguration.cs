using AuthorizationService.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthorizationService.DAL.Contexts.Configurations;

public class PassportConfiguration : IEntityTypeConfiguration<Passport>
{
    public void Configure(EntityTypeBuilder<Passport> builder)
    {
        builder
            .HasKey(p => p.Id);

        builder
            .HasAlternateKey(p => p.IdentificationNumber);

        builder
            .HasAlternateKey(p => new { p.Series, p.Number });

        builder
            .Property(p => p.Firstname)
            .HasMaxLength(100);

        builder
            .Property(p => p.Lastname)
            .HasMaxLength(100);

        builder
            .Property(p => p.Patronymic)
            .HasMaxLength(100);

        builder
            .Property(p => p.Series)
            .HasMaxLength(2);

        builder
            .Property(p => p.Number)
            .HasMaxLength(7);

        builder
            .Property(p => p.IdentificationNumber)
            .HasMaxLength(14);
    }
}
