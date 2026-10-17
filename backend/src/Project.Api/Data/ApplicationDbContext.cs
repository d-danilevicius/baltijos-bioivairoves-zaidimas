using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.Api.Entities;

namespace Project.Api.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<ActionLog> ActionLogs { get; set; } = null!;

    // Fixed role Ids and ConcurrencyStamps to allow deterministic migrations
    private const string StudentRoleId = "11111111-1111-1111-1111-111111111111";
    private const string TeacherRoleId = "22222222-2222-2222-2222-222222222222";
    private const string AdministratorRoleId = "33333333-3333-3333-3333-333333333333";

    private const string StudentConcurrencyStamp = "a1b2c3d4-0000-0000-0000-000000000001";
    private const string TeacherConcurrencyStamp = "a1b2c3d4-0000-0000-0000-000000000002";
    private const string AdministratorConcurrencyStamp = "a1b2c3d4-0000-0000-0000-000000000003";

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Ensure unique index on NormalizedEmail as required by the plan
        builder.Entity<ApplicationUser>()
            .HasIndex(u => u.NormalizedEmail)
            .IsUnique();

        // Seed roles with fixed ids and concurrency stamps
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = StudentRoleId,
                Name = RoleNames.Student,
                NormalizedName = RoleNames.Student.ToUpperInvariant(),
                ConcurrencyStamp = StudentConcurrencyStamp
            },
            new IdentityRole
            {
                Id = TeacherRoleId,
                Name = RoleNames.Teacher,
                NormalizedName = RoleNames.Teacher.ToUpperInvariant(),
                ConcurrencyStamp = TeacherConcurrencyStamp
            },
            new IdentityRole
            {
                Id = AdministratorRoleId,
                Name = RoleNames.Administrator,
                NormalizedName = RoleNames.Administrator.ToUpperInvariant(),
                ConcurrencyStamp = AdministratorConcurrencyStamp
            }
        );

        // ActionLog entity configuration
        builder.Entity<ActionLog>(e =>
        {
            e.Property(a => a.Action).HasMaxLength(50).IsRequired();
            e.HasIndex(a => a.OccurredAtUtc);

            // Nullable FK to AspNetUsers; when a user is deleted the UserId is set to null
            e.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
