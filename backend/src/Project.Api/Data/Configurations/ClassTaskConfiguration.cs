using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Api.Entities;

namespace Project.Api.Data.Configurations;

public class ClassTaskConfiguration : IEntityTypeConfiguration<ClassTask>
{
    public void Configure(EntityTypeBuilder<ClassTask> builder)
    {
        builder.ToTable("ClassTasks");
        builder.Property(t => t.Type).HasConversion<string>().HasMaxLength(30);
        builder.Property(t => t.Question).HasMaxLength(500).IsRequired();
        builder.Property(t => t.CorrectAnswer).HasMaxLength(200).IsRequired();

        builder.HasIndex(t => t.ClassId);

        // When the class is deleted, its tasks are deleted too (VT-009)
        builder.HasOne<SchoolClass>()
            .WithMany()
            .HasForeignKey(t => t.ClassId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Observation>()
            .WithMany()
            .HasForeignKey(t => t.ObservationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
