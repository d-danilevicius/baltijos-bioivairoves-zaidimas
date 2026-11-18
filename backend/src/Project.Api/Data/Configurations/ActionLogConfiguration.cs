using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Api.Entities;

namespace Project.Api.Data.Configurations;

public class ActionLogConfiguration : IEntityTypeConfiguration<ActionLog>
{
    public void Configure(EntityTypeBuilder<ActionLog> builder)
    {
        builder.Property(a => a.Action).HasMaxLength(50).IsRequired();
        builder.HasIndex(a => a.OccurredAtUtc);

        // When a user is deleted, the UserId is set to null
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
