using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Api.Entities;

namespace Project.Api.Data.Configurations;

public class SchoolClassConfiguration : IEntityTypeConfiguration<SchoolClass>
{
    public void Configure(EntityTypeBuilder<SchoolClass> builder)
    {
        builder.ToTable("Classes");
        builder.Property(c => c.Name).HasMaxLength(50).IsRequired();
        builder.Property(c => c.Code).HasMaxLength(6).IsFixedLength().IsRequired();
        builder.HasIndex(c => c.Code).IsUnique();

        // The season is empty until the GBIF tasks are created; a second request finds it set and fails
        builder.Property(c => c.Season).HasConversion<string>().HasMaxLength(10).IsConcurrencyToken();

        // When the teacher account is deleted, the classes are deleted too
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(c => c.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
