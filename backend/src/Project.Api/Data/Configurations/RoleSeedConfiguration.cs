using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Api.Entities;

namespace Project.Api.Data.Configurations;

public class RoleSeedConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    // Fixed Ids and ConcurrencyStamps keep migrations deterministic
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            CreateRole("11111111-1111-1111-1111-111111111111", RoleNames.Student, "a1b2c3d4-0000-0000-0000-000000000001"),
            CreateRole("22222222-2222-2222-2222-222222222222", RoleNames.Teacher, "a1b2c3d4-0000-0000-0000-000000000002"),
            CreateRole("33333333-3333-3333-3333-333333333333", RoleNames.Administrator, "a1b2c3d4-0000-0000-0000-000000000003"));
    }

    private static IdentityRole CreateRole(string id, string name, string concurrencyStamp) => new()
    {
        Id = id,
        Name = name,
        NormalizedName = name.ToUpperInvariant(),
        ConcurrencyStamp = concurrencyStamp
    };
}
