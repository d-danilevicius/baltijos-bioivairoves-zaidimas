using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.Api.Entities;

namespace Project.Api.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<ActionLog> ActionLogs { get; set; }

    public DbSet<SchoolClass> Classes { get; set; }

    public DbSet<ClassMembership> ClassMemberships { get; set; }

    public DbSet<Observation> Observations { get; set; }

    public DbSet<ClassTask> ClassTasks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
