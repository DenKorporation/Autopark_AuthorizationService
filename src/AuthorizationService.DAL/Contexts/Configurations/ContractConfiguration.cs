using AuthorizationService.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthorizationService.DAL.Contexts.Configurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.Number)
            .HasMaxLength(20);
    }
}
