using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Api.Entities;

namespace Project.Api.Data.Configurations;

public class ClassMembershipConfiguration : IEntityTypeConfiguration<ClassMembership>
{
    public void Configure(EntityTypeBuilder<ClassMembership> builder)
    {
        builder.ToTable("ClassMemberships");

        // A student has only one active membership (VT-025)
        builder.HasIndex(m => m.StudentId)
            .IsUnique()
            .HasFilter("\"EndedAtUtc\" IS NULL");

        builder.HasIndex(m => m.ClassId);

        // When the student account is deleted, the memberships are deleted too
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(m => m.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // When the class is deleted, the memberships are deleted too
        builder.HasOne<SchoolClass>()
            .WithMany()
            .HasForeignKey(m => m.ClassId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
