using AuthorizationService.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthorizationService.DAL.Contexts.Configurations;

public class WorkBookConfiguration : IEntityTypeConfiguration<WorkBook>
{
    public void Configure(EntityTypeBuilder<WorkBook> builder)
    {
        builder
            .HasKey(wb => wb.Id);

        builder
            .Property(wb => wb.Number)
            .HasMaxLength(20);
    }
}
