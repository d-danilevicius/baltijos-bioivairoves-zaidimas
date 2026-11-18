using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Api.Entities;

namespace Project.Api.Data.Configurations;

public class ObservationConfiguration : IEntityTypeConfiguration<Observation>
{
    public void Configure(EntityTypeBuilder<Observation> builder)
    {
        builder.ToTable("Observations");
        builder.Property(o => o.Species).HasMaxLength(200).IsRequired();
        builder.Property(o => o.GbifClass).HasMaxLength(100);
        builder.Property(o => o.Family).HasMaxLength(100);
        builder.Property(o => o.Locality).HasMaxLength(200);
        builder.Property(o => o.DatasetName).HasMaxLength(300);
        builder.Property(o => o.License).HasMaxLength(200);
        builder.Property(o => o.IucnCategory).HasMaxLength(10);
        builder.Property(o => o.Season).HasConversion<string>().HasMaxLength(10);

        // One GBIF record is stored once for a class
        builder.HasIndex(o => new { o.ClassId, o.GbifKey }).IsUnique();

        // When the class is deleted, its observations are deleted too
        builder.HasOne<SchoolClass>()
            .WithMany()
            .HasForeignKey(o => o.ClassId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
