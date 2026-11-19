using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Api.Entities;

namespace Project.Api.Data.Configurations;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable("Submissions");
        builder.Property(s => s.Answer).HasMaxLength(100).IsRequired();
        builder.Property(s => s.Evidence).HasMaxLength(500).IsRequired();

        // A student submits a task only once; a deleted task leaves an empty value, which the index allows many times (FR-005)
        builder.HasIndex(s => new { s.ClassTaskId, s.StudentId })
            .IsUnique();

        // When the task is deleted, the student's submission stays (VT-018)
        builder.HasOne<ClassTask>()
            .WithMany()
            .HasForeignKey(s => s.ClassTaskId)
            .OnDelete(DeleteBehavior.SetNull);

        // When the student account is deleted, the submissions are deleted too
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(s => s.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
